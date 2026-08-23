using System.Net;

using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Infrastructure.Persistence;
using Licitaciones.IntegrationTests.Common;
using Licitaciones.IntegrationTests.Proveedores;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Licitaciones.IntegrationTests.Hu23;

[Collection(PostgreSqlCollection.Name)]
public sealed class CrudWebFormulariosInvalidosTests
{
    private readonly PostgreSqlFixture _database;

    public CrudWebFormulariosInvalidosTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-23")]
    public async Task Formulario_Proveedores_DatosInvalidos_DebeMostrarErrorJuntoAlCampoYConservarDatos()
    {
        var run = Token();
        var nombreDuplicado = $"HU23{run} duplicado";
        await CrearProveedorAsync(nombreDuplicado);
        var nombreEnviado = nombreDuplicado.ToUpperInvariant();

        await using var factory = CrearWebFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.PostAsync(
            "/Proveedores/Create",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Nombre"] = nombreEnviado
            }));

        Assert.True(response.IsSuccessStatusCode,
            "Con datos inválidos el formulario debe re-renderizarse (200), nunca aceptarse por redirección.");
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("field-validation-error", html, StringComparison.Ordinal);
        Assert.Contains(
            "Ya existe un proveedor con ese nombre.",
            html,
            StringComparison.Ordinal);
        Assert.Contains(
            $"value=\"{nombreEnviado}\"",
            html,
            StringComparison.Ordinal);
    }

    [Fact]
    [Trait("HU", "HU-23")]
    public async Task Formulario_Licitaciones_DatosInvalidos_DebeMostrarErrorJuntoAlCampoYConservarDatos()
    {
        var run = Token();
        await using var factory = CrearWebFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.PostAsync(
            "/Licitaciones/Create",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Codigo"] = $"HU23F{run.ToUpperInvariant()}",
                ["Titulo"] = $"Presupuesto invalido {run}",
                ["Presupuesto"] = "0",
                ["FechaCierre"] = "2027-01-31T10:00"
            }));

        Assert.True(response.IsSuccessStatusCode,
            "Con datos inválidos el formulario debe re-renderizarse (200), nunca aceptarse por redirección.");
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("field-validation-error", html, StringComparison.Ordinal);
        Assert.Contains("data-valmsg-for=\"Presupuesto\"", html, StringComparison.Ordinal);
        Assert.Contains($"value=\"HU23F{run.ToUpperInvariant()}\"", html, StringComparison.Ordinal);
        Assert.Contains($"value=\"Presupuesto invalido {run}\"", html, StringComparison.Ordinal);
    }

    [Fact]
    [Trait("HU", "HU-23")]
    public async Task Formulario_Ofertas_DatosInvalidos_DebeMostrarErrorJuntoAlCampoYConservarDatos()
    {
        var run = Token();
        var licitacionId = await SembrarLicitacionPublicadaAsync(run);
        var proveedorId = await CrearProveedorAsync($"Proveedor HU23{run} formulario");

        await using var factory = CrearWebFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.PostAsync(
            "/Ofertas/Create",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["LicitacionId"] = licitacionId.ToString(),
                ["ProveedorId"] = proveedorId.ToString(),
                ["Monto"] = "0"
            }));

        Assert.True(response.IsSuccessStatusCode,
            "Con datos inválidos el formulario de ofertas debe re-renderizarse (200).");
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("field-validation-error", html, StringComparison.Ordinal);
        Assert.Contains("data-valmsg-for=\"Monto\"", html, StringComparison.Ordinal);
        Assert.Contains(
            $"value=\"{licitacionId}\"",
            html,
            StringComparison.Ordinal);
        Assert.Contains(
            $"value=\"{proveedorId}\"",
            html,
            StringComparison.Ordinal);
    }

    [Fact]
    [Trait("HU", "HU-23")]
    public async Task Formulario_NivelesAprobacion_DatosInvalidos_DebeMostrarErrorJuntoAlCampoYConservarDatos()
    {
        var run = Token();

        await using var factory = CrearWebFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.PostAsync(
            "/NivelesAprobacion/Create",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Nombre"] = $"Rango invertido {run}",
                ["MontoMinimo"] = "100",
                ["MontoMaximo"] = "50"
            }));

        Assert.True(response.IsSuccessStatusCode,
            "Con datos inválidos el formulario de niveles debe re-renderizarse (200).");
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("field-validation-error", html, StringComparison.Ordinal);
        Assert.Contains("data-valmsg-for=\"MontoMaximo\"", html, StringComparison.Ordinal);
        Assert.Contains($"value=\"Rango invertido {run}\"", html, StringComparison.Ordinal);
        Assert.Contains("value=\"100\"", html, StringComparison.Ordinal);
    }

    [Fact]
    [Trait("HU", "HU-23")]
    public async Task Formulario_TiposCambio_DatosInvalidos_DebeMostrarErrorJuntoAlCampoYConservarDatos()
    {
        var run = Token();

        await using var factory = CrearWebFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.PostAsync(
            "/TiposCambio/Create",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Valor"] = "0",
                ["Fecha"] = "2026-03-15"
            }));

        Assert.True(response.IsSuccessStatusCode,
            "Con datos inválidos el formulario de tipos de cambio debe re-renderizarse (200).");
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("field-validation-error", html, StringComparison.Ordinal);
        Assert.Contains("data-valmsg-for=\"Valor\"", html, StringComparison.Ordinal);
        Assert.Contains("value=\"2026-03-15\"", html, StringComparison.Ordinal);
    }

    private static string Token() => Guid.NewGuid().ToString("N")[..8];

    private async Task<Guid> SembrarLicitacionPublicadaAsync(string run)
    {
        await using var context = _database.CrearContexto();
        var licitacion = LicitacionTestHelper.PublicarLicitacion(
            $"HU23FRM{run.ToUpperInvariant()}",
            DateTimeOffset.UtcNow.AddDays(7));
        context.Licitaciones.Add(licitacion);
        await context.SaveChangesAsync();
        return licitacion.Id;
    }

    private async Task<Guid> CrearProveedorAsync(string nombre)
    {
        await using var context = _database.CrearContexto();
        return (await new CrearProveedorService(new ProveedorRepository(context))
            .CrearAsync(new CrearProveedorRequest(nombre))).Id;
    }

    private WebApplicationFactory<Licitaciones.Web.Controllers.ProveedoresController>
        CrearWebFactory()
    {
        return new WebApplicationFactory<Licitaciones.Web.Controllers.ProveedoresController>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, configuration) =>
                    configuration.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Database:ApplyMigrationsOnStartup"] = "false"
                    }));
                builder.ConfigureLogging(logging => logging.ClearProviders());
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<LicitacionesDbContext>>();
                    services.RemoveAll<LicitacionesDbContext>();
                    services.AddControllersWithViews(options =>
                        options.Filters.Add(
                            new Microsoft.AspNetCore.Mvc.IgnoreAntiforgeryTokenAttribute
                            {
                                Order = 1001
                            }));
                    services.AddDbContext<LicitacionesDbContext>(options =>
                        options.UseNpgsql(_database.ConnectionString));
                    services.AddDataProtection().UseEphemeralDataProtectionProvider();
                });
            });
    }
}
