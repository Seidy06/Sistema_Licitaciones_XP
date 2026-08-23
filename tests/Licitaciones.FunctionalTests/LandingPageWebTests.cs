using System.Net;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Licitaciones.FunctionalTests;

public sealed class LandingPageWebTests
{
    private const string AgenteMovil =
        "Mozilla/5.0 (Linux; Android 13; Pixel 7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Mobile Safari/537.36";

    private static readonly string[] SeccionesExplicativasEsperadas =
    [
        "propósito de la aplicación",
        "flujo de licitación",
        "ofertas",
        "mejor oferta",
        "nivel de aprobación",
        "conversión monetaria"
    ];

    [Fact]
    [Trait("HU", "HU-20")]
    public async Task Raiz_SinAutenticacion_DebeMostrarLandingConSeccionesExplicativas()
    {
        var (statusCode, html, rutaFinal) = await ObtenerLandingAsync();

        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.True(
            rutaFinal == "/",
            $"El acceso anónimo a '/' no debe redirigir a autenticación; terminó en '{rutaFinal}'.");

        foreach (var seccion in SeccionesExplicativasEsperadas)
        {
            Assert.True(
                html.Contains(seccion, StringComparison.OrdinalIgnoreCase),
                $"La landing debe explicar la sección \"{seccion}\".");
        }
    }

    [Fact]
    [Trait("HU", "HU-20")]
    public async Task Landing_ConDispositivoMovil_DebeSerResponsiva()
    {
        var (statusCode, html, _) = await ObtenerLandingAsync(AgenteMovil);

        Assert.Equal(HttpStatusCode.OK, statusCode);

        var viewport = Regex.Match(
            html,
            "<meta\\s+[^>]*name=\"viewport\"[^>]*>",
            RegexOptions.IgnoreCase);
        Assert.True(viewport.Success, "La landing debe declarar la meta etiqueta viewport.");
        Assert.Contains(
            "width=device-width",
            viewport.Value,
            StringComparison.OrdinalIgnoreCase);

        Assert.Matches(
            "<link\\s+[^>]*href=\"[^\"]*bootstrap[^\"]*\\.css[^\"]*\"",
            html);

        var contenidoPrincipal = ExtraerContenidoPrincipal(html);
        var columnasResponsivas = Regex.Matches(
            contenidoPrincipal,
            "\\bcol-(?:xs|sm|md|lg|xl)-?\\d*\\b",
            RegexOptions.IgnoreCase);
        Assert.True(
            columnasResponsivas.Count >= 3,
            $"El contenido de la landing debe usar la rejilla responsiva de Bootstrap " +
            $"(columnas por punto de ruptura); solo se encontraron {columnasResponsivas.Count}.");
    }

    private static async Task<(HttpStatusCode StatusCode, string Html, string RutaFinal)> ObtenerLandingAsync(
        string? agente = null)
    {
        await using var factory = new WebApplicationFactory<
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
        using var client = factory.CreateClient();
        if (agente is not null)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd(agente);
        }

        var response = await client.GetAsync("/");
        return (
            response.StatusCode,
            await response.Content.ReadAsStringAsync(),
            response.RequestMessage?.RequestUri?.PathAndQuery ?? string.Empty);
    }

    private static string ExtraerContenidoPrincipal(string html)
    {
        var match = Regex.Match(
            html,
            "<main\\b[^>]*>(?<contenido>.*?)</main>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        Assert.True(match.Success, "El layout debe renderizar el cuerpo dentro de <main>.");
        return match.Groups["contenido"].Value;
    }
}
