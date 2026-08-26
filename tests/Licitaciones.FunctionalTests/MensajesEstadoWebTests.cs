using System.Net;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Licitaciones.FunctionalTests;

public sealed class MensajesEstadoWebTests
{
    [Fact]
    [Trait("HU", "HU-25")]
    public async Task PostCrearProveedor_DatosVacios_DebeMostrarAlertaErrorDismissible()
    {
        using var client = await CrearClienteAsync();
        var (token, cookie) = await ObtenerTokenAsync(client, "/Proveedores/Create");

        var post = await PostConTokenAsync(
            client, "/Proveedores/Create", token, cookie,
            new Dictionary<string, string>());
        var html = await post.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, post.StatusCode);

        Assert.Matches(
            "<div\\s+class=\"alert\\s+alert-danger\\s+alert-dismissible",
            html);
    }

    [Fact]
    [Trait("HU", "HU-25")]
    public async Task PostCrearProveedor_DatosVacios_DebeTenerBtnCloseConAriaLabel()
    {
        using var client = await CrearClienteAsync();
        var (token, cookie) = await ObtenerTokenAsync(client, "/Proveedores/Create");

        var post = await PostConTokenAsync(
            client, "/Proveedores/Create", token, cookie,
            new Dictionary<string, string>());
        var html = await post.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, post.StatusCode);

        var btnCloseMatches = Regex.Matches(
            html,
            "<button\\b[^>]*class=\"[^\"]*btn-close[^\"]*\"[^>]*>",
            RegexOptions.IgnoreCase);

        Assert.True(
            btnCloseMatches.Count > 0,
            "Los alertas de error deben incluir un botón de cierre btn-close.");

        foreach (Match match in btnCloseMatches)
        {
            Assert.Contains(
                "aria-label",
                match.Value,
                StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    [Trait("HU", "HU-25")]
    public async Task PostCrearProveedor_DatosVacios_DebeTenerRoleAlert()
    {
        using var client = await CrearClienteAsync();
        var (token, cookie) = await ObtenerTokenAsync(client, "/Proveedores/Create");

        var post = await PostConTokenAsync(
            client, "/Proveedores/Create", token, cookie,
            new Dictionary<string, string>());
        var html = await post.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, post.StatusCode);

        Assert.Contains("role=\"alert\"", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("HU", "HU-25")]
    public async Task PostCrearNivelAprobacion_DatosVacios_DebeMostrarAlertaErrorDismissible()
    {
        using var client = await CrearClienteAsync();
        var (token, cookie) = await ObtenerTokenAsync(client, "/NivelesAprobacion/Create");

        var post = await PostConTokenAsync(
            client, "/NivelesAprobacion/Create", token, cookie,
            new Dictionary<string, string>());
        var html = await post.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, post.StatusCode);

        Assert.Matches(
            "<div\\s+class=\"alert\\s+alert-danger\\s+alert-dismissible",
            html);
    }

    [Fact]
    [Trait("HU", "HU-25")]
    public async Task PostCrearNivelAprobacion_DatosVacios_DebeTenerBtnClose()
    {
        using var client = await CrearClienteAsync();
        var (token, cookie) = await ObtenerTokenAsync(client, "/NivelesAprobacion/Create");

        var post = await PostConTokenAsync(
            client, "/NivelesAprobacion/Create", token, cookie,
            new Dictionary<string, string>());
        var html = await post.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, post.StatusCode);

        var btnClose = Regex.Matches(
            html,
            "<button\\b[^>]*class=\"[^\"]*btn-close[^\"]*\"[^>]*>",
            RegexOptions.IgnoreCase);

        Assert.True(
            btnClose.Count > 0,
            "Los alertas de error deben incluir btn-close.");
    }

    [Fact]
    [Trait("HU", "HU-25")]
    public async Task PostCrearOferta_DatosVacios_DebeMostrarValidacionInline()
    {
        using var client = await CrearClienteAsync();
        var (token, cookie) = await ObtenerTokenAsync(client, "/Ofertas/Create");

        var post = await PostConTokenAsync(
            client, "/Ofertas/Create", token, cookie,
            new Dictionary<string, string>());
        var html = await post.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, post.StatusCode);

        Assert.Contains(
            "text-danger",
            html,
            StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<(string Token, string Cookie)> ObtenerTokenAsync(
        HttpClient client, string pagina)
    {
        var get = await client.GetAsync(pagina);
        var html = await get.Content.ReadAsStringAsync();

        var tokenMatch = Regex.Match(
            html,
            "<input\\b[^>]*name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"",
            RegexOptions.IgnoreCase);
        Assert.True(tokenMatch.Success, $"No se encontró el token antiforgery en '{pagina}'.");

        var cookie = string.Empty;
        if (get.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            foreach (var c in cookies)
            {
                if (c.StartsWith(".AspNetCore.Antiforgery", StringComparison.OrdinalIgnoreCase))
                {
                    cookie = c.Split(';')[0];
                    break;
                }
            }
        }

        return (tokenMatch.Groups[1].Value, cookie);
    }

    private static async Task<HttpResponseMessage> PostConTokenAsync(
        HttpClient client,
        string url,
        string token,
        string cookie,
        Dictionary<string, string> campos)
    {
        var camposConToken = new Dictionary<string, string>(campos)
        {
            ["__RequestVerificationToken"] = token
        };

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new FormUrlEncodedContent(camposConToken)
        };

        if (!string.IsNullOrEmpty(cookie))
        {
            request.Headers.Add("Cookie", cookie);
        }

        return await client.SendAsync(request);
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
