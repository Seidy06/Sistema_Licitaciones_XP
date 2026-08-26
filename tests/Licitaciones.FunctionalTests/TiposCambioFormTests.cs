using System.Net;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Licitaciones.FunctionalTests;

public sealed class TiposCambioFormTests
{
    [Fact]
    [Trait("HU", "HU-16")]
    public async Task Formulario_TiposCambio_Fecha_DebeUsarControlDeCalendario()
    {
        var (statusCode, html) = await ObtenerFormularioCrearAsync();

        Assert.Equal(HttpStatusCode.OK, statusCode);
        var input = BuscarInput(html, "Fecha");
        Assert.Contains("type=\"date\"", input, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("HU", "HU-16")]
    public async Task Formulario_TiposCambio_Fecha_NoDebePermitirTextoLibre()
    {
        var (statusCode, html) = await ObtenerFormularioCrearAsync();

        Assert.Equal(HttpStatusCode.OK, statusCode);
        var input = BuscarInput(html, "Fecha");

        Assert.DoesNotContain("type=\"text\"", input, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("type=\"\"", input, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("HU", "HU-16")]
    public async Task Formulario_TiposCambio_Valor_DebeSerNumericoConMinimo()
    {
        var (statusCode, html) = await ObtenerFormularioCrearAsync();

        Assert.Equal(HttpStatusCode.OK, statusCode);
        var input = BuscarInput(html, "Valor");
        Assert.Contains("type=\"number\"", input, StringComparison.OrdinalIgnoreCase);
        var min = Regex.Match(input, "\\bmin=\"(?<value>[^\"]+)\"", RegexOptions.IgnoreCase);
        Assert.True(min.Success, "El valor debe declarar un mínimo.");
    }

    private static async Task<(HttpStatusCode StatusCode, string Html)> ObtenerFormularioCrearAsync()
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

        var response = await client.GetAsync("/TiposCambio/Create");
        return (response.StatusCode, await response.Content.ReadAsStringAsync());
    }

    private static string BuscarInput(string html, string nombre)
    {
        var match = Regex.Match(
            html,
            $"<input\\b[^>]*\\bname=\"{Regex.Escape(nombre)}\"[^>]*>",
            RegexOptions.IgnoreCase);

        Assert.True(match.Success, $"No se encontró el control para {nombre}.");
        return match.Value;
    }
}
