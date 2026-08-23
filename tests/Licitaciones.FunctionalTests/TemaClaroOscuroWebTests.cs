using System.Net;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Licitaciones.FunctionalTests;

public sealed class TemaClaroOscuroWebTests
{
    public static TheoryData<string> PaginasDelSitio => new()
    {
        "/",
        "/Home/Privacy",
        "/Licitaciones/Create"
    };

    [Theory]
    [Trait("HU", "HU-22")]
    [MemberData(nameof(PaginasDelSitio))]
    public async Task Layout_CualquierPagina_DebeMostrarControlVisibleParaAlternarTema(string pagina)
    {
        using var client = await CrearClienteAsync();

        var response = await client.GetAsync(pagina);
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var encabezado = ExtraerSeccion(html, "<header\\b[^>]*>", "</header>");
        Assert.False(
            string.IsNullOrEmpty(encabezado),
            $"La página '{pagina}' debe renderizar el encabezado con la navegación global.");

        var control = Regex.Match(
            encabezado,
            "<(?:button|a|input)\\b[^>]*\\bid=\"theme-toggle\"[^>]*>",
            RegexOptions.IgnoreCase);
        Assert.True(
            control.Success,
            $"El encabezado de la página '{pagina}' debe incluir un control visible con id 'theme-toggle' para alternar entre modo claro y oscuro.");

        var etiquetaAccesible = Regex.Match(
            control.Value,
            "aria-label=\"([^\"]*)\"",
            RegexOptions.IgnoreCase);
        Assert.True(
            etiquetaAccesible.Success,
            $"El control de tema de la página '{pagina}' debe declarar aria-label para ser accesible.");
        Assert.True(
            etiquetaAccesible.Groups[1].Value.Contains("tema", StringComparison.OrdinalIgnoreCase)
                || etiquetaAccesible.Groups[1].Value.Contains("modo", StringComparison.OrdinalIgnoreCase),
            "El aria-label del control de tema debe describir su propósito ('tema' o 'modo').");
    }

    [Fact]
    [Trait("HU", "HU-22")]
    public async Task ControlDeTema_AlCambiar_DebePersistirPreferenciaEntreSesionesEnLocalStorage()
    {
        using var client = await CrearClienteAsync();

        var response = await client.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var urlSiteJs = ExtraerRecursoEstatico(html, "js/site\\.js");
        Assert.False(
            string.IsNullOrEmpty(urlSiteJs),
            "La página debe cargar el script '/js/site.js' donde reside la lógica del control de tema.");

        var respuestaScript = await client.GetAsync(urlSiteJs);
        Assert.Equal(HttpStatusCode.OK, respuestaScript.StatusCode);
        var js = await respuestaScript.Content.ReadAsStringAsync();

        Assert.True(
            js.Contains("theme-toggle", StringComparison.Ordinal),
            "site.js debe suscribirse al evento del control 'theme-toggle' para alternar el tema al hacer clic.");

        Assert.True(
            Regex.IsMatch(js, "localStorage\\.setItem\\(\\s*[\"']theme[\"']"),
            "Al cambiar el tema se debe guardar la preferencia con localStorage.setItem('theme', …); sin esto la preferencia no persiste entre sesiones.");

        Assert.Matches("[\"']light[\"']", js);
        Assert.Matches("[\"']dark[\"']", js);
    }

    [Fact]
    [Trait("HU", "HU-22")]
    public async Task NuevaVisita_AlCargarPagina_DebeRespetarUltimoTemaSeleccionado()
    {
        using var client = await CrearClienteAsync();

        var response = await client.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var guionesIniciales = ExtraerGuionesEnLineaDeLaCabecera(html);
        Assert.True(
            Regex.IsMatch(guionesIniciales, "localStorage\\.getItem\\(\\s*[\"']theme[\"']\\s*\\)"),
            "Al cargar la página debe leerse la última preferencia guardada con localStorage.getItem('theme') antes de mostrar la interfaz.");
        Assert.True(
            guionesIniciales.Contains("documentElement", StringComparison.Ordinal),
            "La preferencia leída debe aplicarse al elemento raíz (<html>) para respetar el último tema seleccionado desde el primer render.");
        Assert.True(
            guionesIniciales.Contains("data-bs-theme", StringComparison.OrdinalIgnoreCase)
                || guionesIniciales.Contains("bsTheme", StringComparison.Ordinal)
                || guionesIniciales.Contains("classList", StringComparison.Ordinal),
            "La preferencia debe aplicarse como atributo data-bs-theme o clase CSS sobre el documento.");

        var urlSiteCss = ExtraerRecursoEstatico(html, "css/site\\.css");
        Assert.False(
            string.IsNullOrEmpty(urlSiteCss),
            "La página debe referenciar la hoja de estilos '/css/site.css'.");

        var respuestaCss = await client.GetAsync(urlSiteCss);
        Assert.Equal(HttpStatusCode.OK, respuestaCss.StatusCode);
        var css = await respuestaCss.Content.ReadAsStringAsync();

        Assert.True(
            Regex.IsMatch(css, "\\[\\s*data-bs-theme\\s*=\\s*[\"']dark[\"']\\s*\\]|html\\.dark"),
            "site.css debe definir la paleta del modo oscuro mediante el selector [data-bs-theme='dark'] o html.dark para que el tema guardado sea visible.");
    }

    private static async Task<HttpClient> CrearClienteAsync()
    {
        var factory = new WebApplicationFactory<
            Licitaciones.Web.Controllers.HomeController>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, configuration) =>
                    configuration.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Database:ApplyMigrationsOnStartup"] = "false"
                    }));
                builder.ConfigureLogging(logging => logging.ClearProviders());
                builder.ConfigureServices(services =>
                    services.AddDataProtection().UseEphemeralDataProtectionProvider());
            });
        return factory.CreateClient();
    }

    private static string ExtraerSeccion(string html, string patronApertura, string cierre)
    {
        var coincidencia = Regex.Match(
            html,
            $"{patronApertura}(?<contenido>.*?){cierre}",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);
        return coincidencia.Success ? coincidencia.Groups["contenido"].Value : string.Empty;
    }

    private static string? ExtraerRecursoEstatico(string html, string nombreArchivo)
    {
        var coincidencia = Regex.Match(
            html,
            "<(?:script|link)\\b[^>]*(?:src|href)=\"([^\"]*" + nombreArchivo + "[^\"]*)\"",
            RegexOptions.IgnoreCase);
        return coincidencia.Success ? coincidencia.Groups[1].Value : null;
    }

    private static string ExtraerGuionesEnLineaDeLaCabecera(string html)
    {
        var cabeza = ExtraerSeccion(html, "<head\\b[^>]*>", "</head>");
        var codigo = new System.Text.StringBuilder();
        foreach (Match guion in Regex.Matches(
            cabeza,
            "<script(?![^>]*\\bsrc\\s*=)[^>]*>(?<codigo>.*?)</script>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline))
        {
            _ = codigo.AppendLine(guion.Groups["codigo"].Value);
        }

        return codigo.ToString();
    }
}
