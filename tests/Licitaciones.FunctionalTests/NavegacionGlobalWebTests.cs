using System.Net;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Licitaciones.FunctionalTests;

public sealed class NavegacionGlobalWebTests
{
    private const string PatronHrefInicio = "/(?:|Home(?:/Index)?)";
    private const string PatronHrefLicitaciones = "/Licitaciones(?:/Index)?";
    private const string PatronHrefProveedores = "/Proveedores(?:/Index)?";
    private const string PatronHrefOfertas = "/Ofertas(?:/Index)?";
    private const string PatronHrefNivelesAprobacion = "/NivelesAprobacion(?:/Index)?";
    private const string PatronHrefTiposCambio = "/TiposCambio(?:/Index)?";
    private const string PatronHrefDocumentacionApi = "/swagger(?:/index\\.html)?";

    public static TheoryData<string> PaginasDelSitio => new()
    {
        "/",
        "/Home/Privacy",
        "/Licitaciones/Create"
    };

    [Theory]
    [Trait("HU", "HU-21")]
    [MemberData(nameof(PaginasDelSitio))]
    public async Task Layout_CualquierPagina_DebeMostrarMenuGlobalConTodosLosModulos(string pagina)
    {
        using var client = await CrearClienteAsync();

        var response = await client.GetAsync(pagina);
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(
            Regex.IsMatch(html, @"<nav\b"),
            $"La página '{pagina}' debe renderizar el menú de navegación dentro del layout.");

        AssertTieneAncla(html, pagina, "Inicio", PatronHrefInicio);
        AssertTieneAncla(html, pagina, "Licitaciones", PatronHrefLicitaciones);
        AssertTieneAncla(html, pagina, "Proveedores", PatronHrefProveedores);
        AssertTieneAncla(html, pagina, "Ofertas", PatronHrefOfertas);
        AssertTieneAncla(html, pagina, "NivelesAprobacion", PatronHrefNivelesAprobacion);
        AssertTieneAncla(html, pagina, "TiposCambio", PatronHrefTiposCambio);
        AssertTieneAncla(html, pagina, "Documentación de API", PatronHrefDocumentacionApi);
    }

    [Fact]
    [Trait("HU", "HU-21")]
    public async Task Layout_EnPaginaInicio_DebeResaltarSeccionActiva()
    {
        using var client = await CrearClienteAsync();

        var response = await client.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(
            AnclaTieneClaseActiva(html, PatronHrefInicio),
            "El elemento del menú correspondiente a Inicio debe resaltarse como sección activa.");
        Assert.False(
            AnclaTieneClaseActiva(html, PatronHrefLicitaciones),
            "La sección Licitaciones no debe aparecer resaltada cuando se visualiza Inicio.");
    }

    [Fact]
    [Trait("HU", "HU-21")]
    public async Task Layout_EnPaginaDeOtraSeccion_DebeMoverElResaltadoALaSeccionCorrespondiente()
    {
        using var client = await CrearClienteAsync();

        var response = await client.GetAsync("/Licitaciones/Create");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(
            AnclaTieneClaseActiva(html, PatronHrefLicitaciones),
            "El elemento del menú correspondiente a Licitaciones debe resaltarse como sección activa.");
        Assert.False(
            AnclaTieneClaseActiva(html, PatronHrefInicio),
            "La sección Inicio no debe permanecer resaltada cuando se navega a otra sección.");
    }

    [Fact]
    [Trait("HU", "HU-21")]
    public async Task EnlaceADocumentacionApi_DebeAbrirSwaggerUi()
    {
        using var client = await CrearClienteAsync();

        var respuestaMenu = await client.GetAsync("/");
        var htmlMenu = await respuestaMenu.Content.ReadAsStringAsync();
        var hrefDocumentacion = ExtraerHref(htmlMenu, PatronHrefDocumentacionApi);

        Assert.False(
            string.IsNullOrEmpty(hrefDocumentacion),
            "El menú debe incluir un enlace hacia la documentación interactiva de la API (Swagger UI).");

        var respuestaSwagger = await client.GetAsync(hrefDocumentacion);
        var contenidoSwagger = await respuestaSwagger.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, respuestaSwagger.StatusCode);
        Assert.Contains("swagger-ui", contenidoSwagger, StringComparison.OrdinalIgnoreCase);
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

    private static void AssertTieneAncla(string html, string pagina, string modulo, string patronHref)
    {
        Assert.True(
            TieneAnclaConHref(html, patronHref),
            $"El menú de la página '{pagina}' debe contener un enlace al módulo '{modulo}' (href esperado: '{patronHref}').");
    }

    private static bool TieneAnclaConHref(string html, string patronHref) =>
        Regex.IsMatch(html, $@"<a\b[^>]*href=""({patronHref})""", RegexOptions.IgnoreCase);

    private static bool AnclaTieneClaseActiva(string html, string patronHref)
    {
        var claseAntesDeHref = $@"<a\b[^>]*class=""[^""]*\bactive\b[^""]*""[^>]*href=""({patronHref})""";
        var hrefAntesDeClase = $@"<a\b[^>]*href=""({patronHref})""[^>]*class=""[^""]*\bactive\b[^""]*""";
        return Regex.IsMatch(html, claseAntesDeHref, RegexOptions.IgnoreCase)
            || Regex.IsMatch(html, hrefAntesDeClase, RegexOptions.IgnoreCase);
    }

    private static string? ExtraerHref(string html, string patronHref)
    {
        var coincidencia = Regex.Match(
            html,
            $@"<a\b[^>]*href=""({patronHref})""",
            RegexOptions.IgnoreCase);
        return coincidencia.Success ? coincidencia.Groups[1].Value : null;
    }
}
