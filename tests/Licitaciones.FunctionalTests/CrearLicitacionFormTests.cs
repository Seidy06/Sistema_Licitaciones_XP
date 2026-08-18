using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Licitaciones.FunctionalTests;

public sealed class CrearLicitacionFormTests
{
    [Fact]
    [Trait("HU", "HU-10")]
    public async Task Formulario_Presupuesto_DebeUsarControlNumericoQueImpidaValoresNoPositivos()
    {
        var (statusCode, html) = await ObtenerFormularioAsync();

        Assert.Equal(HttpStatusCode.OK, statusCode);
        var input = BuscarInput(html, "Presupuesto");
        Assert.Contains("type=\"number\"", input, StringComparison.OrdinalIgnoreCase);
        var min = Regex.Match(input, "\\bmin=\"(?<value>[^\"]+)\"", RegexOptions.IgnoreCase);
        Assert.True(min.Success, "El presupuesto debe declarar un mínimo positivo en el cliente.");
        Assert.True(
            decimal.Parse(min.Groups["value"].Value, CultureInfo.InvariantCulture) > 0,
            "El mínimo permitido para el presupuesto debe ser mayor que cero.");
    }

    [Fact]
    [Trait("HU", "HU-10")]
    public async Task Formulario_FechaCierre_DebeUsarCalendarioYHoraEnLugarDeTextoLibre()
    {
        var (statusCode, html) = await ObtenerFormularioAsync();

        Assert.Equal(HttpStatusCode.OK, statusCode);
        var input = BuscarInput(html, "FechaCierre");
        Assert.Contains("type=\"datetime-local\"", input, StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<(HttpStatusCode StatusCode, string Html)> ObtenerFormularioAsync()
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

        var response = await client.GetAsync("/Licitaciones/Create");
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
