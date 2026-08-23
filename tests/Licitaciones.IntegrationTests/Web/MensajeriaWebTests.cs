using System.Net;

using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Domain.Aprobaciones;
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

namespace Licitaciones.IntegrationTests.Hu24;

[Collection(PostgreSqlCollection.Name)]
public sealed class MensajeriaWebTests
{
    private readonly PostgreSqlFixture _database;

    public MensajeriaWebTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-24")]
    public async Task Operacion_Exitosa_EliminacionNivel_DebeMostrarAlertaConfirmacionEnDestino()
    {
        var run = Token();
        int nivelId = 0;
        try
        {
            nivelId = await SembrarNivelEliminableAsync(run);

            await using var factory = CrearWebFactory();
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var ejecucion = await client.PostAsync(
                $"/NivelesAprobacion/DeleteConfirmed/{nivelId}",
                new FormUrlEncodedContent([]));

            Assert.Equal(HttpStatusCode.Redirect, ejecucion.StatusCode);
            var destino = ejecucion.Headers.Location;
            Assert.NotNull(destino);

            var paginaDestino = await client.GetAsync(destino.ToString());
            Assert.True(paginaDestino.IsSuccessStatusCode,
                "Tras una operación exitosa la vista destino debe cargarse correctamente.");
            var html = await paginaDestino.Content.ReadAsStringAsync();

            Assert.True(
                html.Contains("alert-success", StringComparison.Ordinal),
                "Una operación exitosa debe mostrar un mensaje de confirmación visible tipo toast/alerta " +
                "(clase alert-success) en la página de destino.");
            Assert.Contains(
                "El nivel de aprobación fue desactivado.",
                html,
                StringComparison.Ordinal);
        }
        finally
        {
            await RestaurarCatalogoNivelesAsync(nivelId);
        }
    }

    [Fact]
    [Trait("HU", "HU-24")]
    public async Task Operacion_Exitosa_RegistroOferta_DebeMostrarAlertaConfirmacionEnListado()
    {
        var run = Token();
        var licitacionId = await SembrarLicitacionPublicadaAsync(run);
        var proveedorId = await CrearProveedorAsync($"Proveedor HU24{run} oferta");

        await using var factory = CrearWebFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var respuesta = await client.PostAsync(
            "/Ofertas/Create",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["LicitacionId"] = licitacionId.ToString(),
                ["ProveedorId"] = proveedorId.ToString(),
                ["Monto"] = "5000"
            }));

        Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
        var destino = respuesta.Headers.Location;
        Assert.NotNull(destino);

        var listado = await client.GetAsync(destino.ToString());
        Assert.True(listado.IsSuccessStatusCode,
            "Tras registrar una oferta el listado debe cargarse correctamente.");
        var html = await listado.Content.ReadAsStringAsync();

        Assert.True(
            html.Contains("alert-success", StringComparison.Ordinal),
            "Registrar una oferta (operación exitosa) debe mostrar un mensaje de confirmación " +
            "visible tipo toast/alerta (clase alert-success) tras la redirección.");
        Assert.Contains(
            "La oferta se registró correctamente.",
            html,
            StringComparison.Ordinal);
    }

    [Fact]
    [Trait("HU", "HU-24")]
    public async Task ErrorNegocio_TraslapeNiveles_DebeMostrarAlertaConMensajeEspecificoSinStacktrace()
    {
        var run = Token();

        await using var factory = CrearWebFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var respuesta = await client.PostAsync(
            "/NivelesAprobacion/Create",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Nombre"] = $"Traslapado HU24{run}",
                ["MontoMinimo"] = "2000000",
                ["MontoMaximo"] = "5000000"
            }));

        Assert.True(respuesta.IsSuccessStatusCode,
            "Un error de negocio no debe romper la aplicación; el formulario debe re-renderizarse (200).");
        var html = await respuesta.Content.ReadAsStringAsync();

        Assert.True(
            html.Contains("alert-danger", StringComparison.Ordinal),
            "Un error de negocio debe mostrarse como componente de mensajería de error visible " +
            "(clase alert-danger), no solo como texto suelto.");

        Assert.True(
            html.Contains("traslape", StringComparison.OrdinalIgnoreCase),
            "El mensaje de error debe ser específico y comprensible sobre la regla violada " +
            "(traslape de rangos de niveles de aprobación).");

        Assert.DoesNotContain("StackTrace", html, StringComparison.Ordinal);
        Assert.DoesNotContain("at Licitaciones.", html, StringComparison.Ordinal);
        Assert.DoesNotContain("An unhandled exception occurred", html, StringComparison.Ordinal);
    }

    private static string Token() => Guid.NewGuid().ToString("N")[..8];

    private async Task<int> SembrarNivelEliminableAsync(string run)
    {
        await using var context = _database.CrearContexto();

        await context.Database.ExecuteSqlRawAsync(
            "UPDATE \"NivelesAprobacion\" SET \"Activo\" = {0} WHERE \"Id\" = {1}",
            false, 3);

        var nivel = NivelAprobacion.Crear(
            $"Nivel eliminable HU24{run}",
            11_000_000m,
            12_000_000m);
        context.NivelesAprobacion.Add(nivel);
        await context.SaveChangesAsync();
        return nivel.Id;
    }

    private async Task RestaurarCatalogoNivelesAsync(int nivelId)
    {
        await using var context = _database.CrearContexto();
        if (nivelId > 0)
        {
            await context.Database.ExecuteSqlRawAsync(
                "DELETE FROM \"NivelesAprobacion\" WHERE \"Id\" = {0}",
                nivelId);
        }

        await context.Database.ExecuteSqlRawAsync(
            "UPDATE \"NivelesAprobacion\" SET \"Activo\" = {0} WHERE \"Id\" = {1}",
            true, 3);
    }

    private async Task<Guid> SembrarLicitacionPublicadaAsync(string run)
    {
        await using var context = _database.CrearContexto();
        var licitacion = LicitacionTestHelper.PublicarLicitacion(
            $"HU24MSG{run.ToUpperInvariant()}",
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
