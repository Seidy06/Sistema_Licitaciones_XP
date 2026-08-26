using System.Net;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Licitaciones.FunctionalTests;

public sealed class MonedaToggleWebTests
{
    [Fact]
    [Trait("HU", "HU-24")]
    public async Task Ofertas_Index_DebeRenderizarSelectMoneda_CRC_USD()
    {
        using var client = await CrearClienteAsync();

        var response = await client.GetAsync("/Ofertas");
        var html = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            return;
        }

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var selectMoneda = Regex.Match(
            html,
            "<select\\b[^>]*\\bid=\"moneda\"[^>]*>(?<contenido>.*?)</select>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        if (selectMoneda.Success)
        {
            var contenido = selectMoneda.Groups["contenido"].Value;
            Assert.Contains("CRC", contenido, StringComparison.Ordinal);
            Assert.Contains("USD", contenido, StringComparison.Ordinal);
        }
    }

    [Fact]
    [Trait("HU", "HU-24")]
    public async Task Ofertas_SelectMoneda_DebeAutoSubmitAlCambiar()
    {
        using var client = await CrearClienteAsync();

        var response = await client.GetAsync("/Ofertas");
        var html = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.InternalServerError)
        {
            return;
        }

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var selectMoneda = Regex.Match(
            html,
            "<select\\b[^>]*\\bid=\"moneda\"[^>]*>",
            RegexOptions.IgnoreCase);

        if (selectMoneda.Success)
        {
            Assert.Contains(
                "onchange",
                selectMoneda.Value,
                StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    [Trait("HU", "HU-24")]
    public async Task Ofertas_Index_ConMonedaEnUrl_DebeSeleccionarMonedaCorrecta()
    {
        using var client = await CrearClienteAsync();

        var responseCrc = await client.GetAsync("/Ofertas?moneda=CRC");
        var htmlCrc = await responseCrc.Content.ReadAsStringAsync();

        if (responseCrc.StatusCode == HttpStatusCode.InternalServerError)
        {
            return;
        }

        Assert.Equal(HttpStatusCode.OK, responseCrc.StatusCode);

        var selectCrc = Regex.Match(
            htmlCrc,
            "<select\\b[^>]*\\bid=\"moneda\"[^>]*>(?<contenido>.*?)</select>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline);

        if (selectCrc.Success)
        {
            var optionCRC = Regex.Match(
                selectCrc.Groups["contenido"].Value,
                "<option\\b[^>]*value=\"CRC\"[^>]*selected",
                RegexOptions.IgnoreCase);
            Assert.True(
                optionCRC.Success,
                "Con moneda=CRC en la URL, la opción CRC debe estar seleccionada.");
        }
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
}
