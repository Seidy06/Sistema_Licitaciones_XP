using Licitaciones.Application.Ofertas.Crear;
using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Domain.Licitaciones;
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

namespace Licitaciones.IntegrationTests.Hu25;

[Collection(PostgreSqlCollection.Name)]
public sealed class FormatoMonetarioWebTests
{
    private const string MontoEsperadoPresupuesto = "\u20A11.500.000,00";
    private const string MontoEsperadoOferta = "\u20A11.250.500,00";
    private const string MontoEsperadoNivelMinimo = "\u20A123.456.789,00";
    private const string MontoEsperadoNivelMaximo = "\u20A124.654.321,00";

    private readonly PostgreSqlFixture _database;

    public FormatoMonetarioWebTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-25")]
    public async Task Listado_Licitaciones_DebePresentarPresupuestoConFormatoEsCR()
    {
        var run = Token();
        var fechaCierre = DateTimeOffset.UtcNow.AddDays(7);

        await using var context = _database.CrearContexto();
        var licitacion = Licitacion.Crear(
            $"HU25FMT{run.ToUpperInvariant()}",
            "Licitación con presupuesto para formato es-CR",
            1_500_000m,
            fechaCierre);
        licitacion.Publicar(new LicitacionTestHelper.FixedClock(fechaCierre.AddDays(-5)));
        context.Licitaciones.Add(licitacion);
        await context.SaveChangesAsync();

        await using var factory = CrearWebFactory();
        using var client = factory.CreateClient();

        var respuesta = await client.GetAsync("/Licitaciones");
        var html = await respuesta.Content.ReadAsStringAsync();

        Assert.True(
            respuesta.IsSuccessStatusCode,
            "El listado de licitaciones debe cargarse correctamente.");
        Assert.Contains(MontoEsperadoPresupuesto, html, StringComparison.Ordinal);
    }

    [Fact]
    [Trait("HU", "HU-25")]
    public async Task Listado_Ofertas_DebePresentarMontoConFormatoEsCR()
    {
        var run = Token();
        var fechaCierre = DateTimeOffset.UtcNow.AddDays(7);
        Guid proveedorId;
        Guid licitacionId;

        await using (var context = _database.CrearContexto())
        {
            var licitacion = Licitacion.Crear(
                $"HU25OFR{run.ToUpperInvariant()}",
                "Licitación con oferta para formato es-CR",
                2_000_000m,
                fechaCierre);
            licitacion.Publicar(new LicitacionTestHelper.FixedClock(fechaCierre.AddDays(-5)));
            context.Licitaciones.Add(licitacion);
            await context.SaveChangesAsync();
            licitacionId = licitacion.Id;

            proveedorId = (await new CrearProveedorService(new ProveedorRepository(context))
                .CrearAsync(new CrearProveedorRequest($"Proveedor HU25{run}"))).Id;

            await new CrearOfertaService(new OfertaRepository(context), new LicitacionTestHelper.FixedClock(fechaCierre.AddDays(-1)))
                .CrearAsync(new CrearOfertaRequest(licitacionId, proveedorId, 1_250_500m));
        }

        await using var factory = CrearWebFactory();
        using var client = factory.CreateClient();

        var respuesta = await client.GetAsync($"/Ofertas?licitacionId={licitacionId}");
        var html = await respuesta.Content.ReadAsStringAsync();

        Assert.True(
            respuesta.IsSuccessStatusCode,
            "El listado de ofertas debe cargarse correctamente.");
        Assert.Contains(MontoEsperadoOferta, html, StringComparison.Ordinal);
    }

    [Fact]
    [Trait("HU", "HU-25")]
    public async Task Listado_NivelesAprobacion_DebePresentarMontosConFormatoEsCR()
    {
        var run = Token();
        int nivelId;

        await using (var context = _database.CrearContexto())
        {
            await context.Database.ExecuteSqlRawAsync(
                "UPDATE \"NivelesAprobacion\" SET \"Activo\" = {0} WHERE \"Id\" = {1}",
                false, 3);

            var nivel = Domain.Aprobaciones.NivelAprobacion.Crear(
                $"Nivel formato HU25{run}",
                23_456_789m,
                24_654_321m);
            context.NivelesAprobacion.Add(nivel);
            await context.SaveChangesAsync();
            nivelId = nivel.Id;
        }

        try
        {
            await using var factory = CrearWebFactory();
            using var client = factory.CreateClient();

            var respuesta = await client.GetAsync("/NivelesAprobacion");
            var html = await respuesta.Content.ReadAsStringAsync();

            Assert.True(
                respuesta.IsSuccessStatusCode,
                "El listado de niveles de aprobación debe cargarse correctamente.");
            Assert.Contains(MontoEsperadoNivelMinimo, html, StringComparison.Ordinal);
            Assert.Contains(MontoEsperadoNivelMaximo, html, StringComparison.Ordinal);
        }
        finally
        {
            await using var context = _database.CrearContexto();
            await context.Database.ExecuteSqlRawAsync(
                "DELETE FROM \"NivelesAprobacion\" WHERE \"Id\" = {0}",
                nivelId);
            await context.Database.ExecuteSqlRawAsync(
                "UPDATE \"NivelesAprobacion\" SET \"Activo\" = {0} WHERE \"Id\" = {1}",
                true, 3);
        }
    }

    private static string Token() => Guid.NewGuid().ToString("N")[..8];

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
                    services.AddDbContext<LicitacionesDbContext>(options =>
                        options.UseNpgsql(_database.ConnectionString));
                    services.AddDataProtection().UseEphemeralDataProtectionProvider();
                });
            });
    }
}
