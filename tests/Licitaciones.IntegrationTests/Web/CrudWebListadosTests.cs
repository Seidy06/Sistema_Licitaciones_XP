using Licitaciones.Application.Proveedores;
using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Domain.Aprobaciones;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;
using Licitaciones.Infrastructure.Persistence;
using Licitaciones.Infrastructure.Time;
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
public sealed class CrudWebListadosTests
{
    private const string TablaHtml = "<table";

    private readonly PostgreSqlFixture _database;

    public CrudWebListadosTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-23")]
    public async Task Listado_Proveedores_DebeSoportarPaginacionFiltroYOrden()
    {
        var run = Token();
        await CrearProveedorAsync($"HU23{run} AAA");
        await CrearProveedorAsync($"HU23{run} BBB");
        await CrearProveedorAsync($"HU23{run} CCC");

        await using var factory = CrearWebFactory();
        using var client = factory.CreateClient();

        var pagina1 = await client.GetStringAsync(
            $"/Proveedores?nombre=HU23{run}&pagina=1&tamanoPagina=2&ordenarPor=Nombre&descendente=false");

        Assert.Contains(TablaHtml, pagina1, StringComparison.Ordinal);
        Assert.Contains("AAA", pagina1, StringComparison.Ordinal);
        Assert.Contains("BBB", pagina1, StringComparison.Ordinal);
        Assert.DoesNotContain("CCC", pagina1, StringComparison.Ordinal);

        var pagina2 = await client.GetStringAsync(
            $"/Proveedores?nombre=HU23{run}&pagina=2&tamanoPagina=2&ordenarPor=Nombre&descendente=false");

        Assert.Contains("CCC", pagina2, StringComparison.Ordinal);
        Assert.DoesNotContain("AAA", pagina2, StringComparison.Ordinal);

        var descendente = await client.GetStringAsync(
            $"/Proveedores?nombre=HU23{run}&pagina=1&tamanoPagina=10&ordenarPor=Nombre&descendente=true");

        Assert.True(
            Posicion(descendente, "CCC") < Posicion(descendente, "BBB")
                && Posicion(descendente, "BBB") < Posicion(descendente, "AAA"),
            "El listado debe respetar el orden descendente solicitado.");
    }

    [Fact]
    [Trait("HU", "HU-23")]
    public async Task Listado_Licitaciones_DebeSoportarPaginacionFiltroYOrden()
    {
        var run = Token();
        await SembrarLicitacionAsync(run, "alfa");
        await SembrarLicitacionAsync(run, "beta");
        await SembrarLicitacionAsync(run, "gamma");

        await using var factory = CrearWebFactory();
        using var client = factory.CreateClient();

        var pagina1 = await client.GetStringAsync(
            $"/Licitaciones?codigo=HU23{run}&pagina=1&tamanoPagina=2&ordenarPor=codigo&descendente=false");

        Assert.True(pagina1.Contains(TablaHtml, StringComparison.Ordinal),
            $"El listado de licitaciones debe renderizar una tabla. Status esperado 200.");
        Assert.Contains("alfa", pagina1, StringComparison.Ordinal);
        Assert.Contains("beta", pagina1, StringComparison.Ordinal);
        Assert.DoesNotContain("gamma", pagina1, StringComparison.Ordinal);

        var pagina2 = await client.GetStringAsync(
            $"/Licitaciones?codigo=HU23{run}&pagina=2&tamanoPagina=2&ordenarPor=codigo&descendente=false");

        Assert.Contains("gamma", pagina2, StringComparison.Ordinal);
        Assert.DoesNotContain("alfa", pagina2, StringComparison.Ordinal);

        var descendente = await client.GetStringAsync(
            $"/Licitaciones?codigo=HU23{run}&pagina=1&tamanoPagina=10&ordenarPor=codigo&descendente=true");

        Assert.True(
            Posicion(descendente, "gamma") < Posicion(descendente, "alfa"),
            "El listado de licitaciones debe respetar el orden descendente solicitado.");
    }

    [Fact]
    [Trait("HU", "HU-23")]
    public async Task Listado_Ofertas_DebeSoportarPaginacionFiltroYOrden()
    {
        var run = Token();
        var licitacionId = await SembrarLicitacionConOfertasAsync(run);

        await using var factory = CrearWebFactory();
        using var client = factory.CreateClient();

        var pagina1 = await client.GetStringAsync(
            $"/Ofertas?licitacionId={licitacionId}&proveedor=HU23{run}&pagina=1&tamanoPagina=2&ordenarPor=monto&descendente=false");

        Assert.Contains(TablaHtml, pagina1, StringComparison.Ordinal);
        Assert.Contains("uno", pagina1, StringComparison.Ordinal);
        Assert.Contains("dos", pagina1, StringComparison.Ordinal);
        Assert.DoesNotContain("tres", pagina1, StringComparison.Ordinal);

        var pagina2 = await client.GetStringAsync(
            $"/Ofertas?licitacionId={licitacionId}&proveedor=HU23{run}&pagina=2&tamanoPagina=2&ordenarPor=monto&descendente=false");

        Assert.Contains("tres", pagina2, StringComparison.Ordinal);
        Assert.DoesNotContain("uno", pagina2, StringComparison.Ordinal);

        var completa = await client.GetStringAsync(
            $"/Ofertas?licitacionId={licitacionId}&proveedor=HU23{run}&pagina=1&tamanoPagina=10&ordenarPor=monto&descendente=true");

        Assert.True(
            Posicion(completa, "tres") < Posicion(completa, "uno"),
            "El listado de ofertas debe respetar el orden descendente por monto.");
    }

    [Fact]
    [Trait("HU", "HU-23")]
    public async Task Listado_NivelesAprobacion_DebeSoportarPaginacionFiltroYOrden()
    {
        await using var factory = CrearWebFactory();
        using var client = factory.CreateClient();

        var pagina1 = await client.GetStringAsync(
            "/NivelesAprobacion?nombre=tivo&pagina=1&tamanoPagina=1&ordenarPor=montoMinimo&descendente=false");

        Assert.Contains(TablaHtml, pagina1, StringComparison.Ordinal);
        Assert.Contains("Operativo", pagina1, StringComparison.Ordinal);
        Assert.DoesNotContain("Directivo", pagina1, StringComparison.Ordinal);

        var pagina2 = await client.GetStringAsync(
            "/NivelesAprobacion?nombre=tivo&pagina=2&tamanoPagina=1&ordenarPor=montoMinimo&descendente=false");

        Assert.Contains("Directivo", pagina2, StringComparison.Ordinal);
        Assert.DoesNotContain("Operativo", pagina2, StringComparison.Ordinal);

        var descendente = await client.GetStringAsync(
            "/NivelesAprobacion?pagina=1&tamanoPagina=50&ordenarPor=montoMinimo&descendente=true");

        Assert.True(
            Posicion(descendente, "Directivo") < Posicion(descendente, "Gerencial")
                && Posicion(descendente, "Gerencial") < Posicion(descendente, "Operativo"),
            "El listado de niveles debe respetar el orden descendente por monto mínimo.");
    }

    [Fact]
    [Trait("HU", "HU-23")]
    public async Task Listado_TiposCambio_DebeSoportarPaginacionYOrden()
    {
        try
        {
            await SembrarTiposCambioHistoricosAsync();

            await using var factory = CrearWebFactory();
            using var client = factory.CreateClient();

            var pagina1 = await client.GetStringAsync(
                "/TiposCambio?pagina=1&tamanoPagina=1&ordenarPor=fecha&descendente=false");

            Assert.Contains(TablaHtml, pagina1, StringComparison.Ordinal);
            Assert.Contains("2000", pagina1, StringComparison.Ordinal);
            Assert.DoesNotContain("2001", pagina1, StringComparison.Ordinal);

            var pagina2 = await client.GetStringAsync(
                "/TiposCambio?pagina=2&tamanoPagina=1&ordenarPor=fecha&descendente=false");

            Assert.Contains("2001", pagina2, StringComparison.Ordinal);
            Assert.DoesNotContain("2000", pagina2, StringComparison.Ordinal);
        }
        finally
        {
            await RestaurarTiposCambioHistoricosAsync();
        }
    }

    private static string Token() => Guid.NewGuid().ToString("N")[..8];

    private static int Posicion(string html, string texto) =>
        html.IndexOf(texto, StringComparison.Ordinal);

    private async Task<Guid> SembrarLicitacionAsync(string run, string sufijo)
    {
        await using var context = _database.CrearContexto();
        var licitacion = Licitacion.Crear(
            $"HU23{run}{sufijo.ToUpperInvariant()}",
            $"Licitación HU-23 {run} {sufijo}",
            100_000m,
            DateTimeOffset.UtcNow.AddDays(30));
        context.Licitaciones.Add(licitacion);
        await context.SaveChangesAsync();
        return licitacion.Id;
    }

    private async Task<Guid> SembrarLicitacionConOfertasAsync(string run)
    {
        await using var context = _database.CrearContexto();
        var licitacion = LicitacionTestHelper.PublicarLicitacion(
            $"HU23OF{run.ToUpperInvariant()}",
            DateTimeOffset.UtcNow.AddDays(7));

        decimal[] montos = [100m, 200m, 300m];
        string[] sufijos = ["uno", "dos", "tres"];
        for (var i = 0; i < montos.Length; i++)
        {
            var proveedor = Licitaciones.Domain.Proveedores.Proveedor.Crear(
                $"Proveedor HU23{run} {sufijos[i]}");
            var oferta = Oferta.Crear(
                licitacion.Id,
                proveedor.Id,
                montos[i],
                new SystemClock());
            context.Proveedores.Add(proveedor);
            context.Ofertas.Add(oferta);
        }

        context.Licitaciones.Add(licitacion);
        await context.SaveChangesAsync();
        return licitacion.Id;
    }

    private async Task SembrarTiposCambioHistoricosAsync()
    {
        await using var context = _database.CrearContexto();
        await context.Database.ExecuteSqlRawAsync("""
            INSERT INTO "TiposCambio"
                ("Id", "MonedaOrigen", "MonedaDestino", "Valor", "Fecha", "Activo", "CreatedAt", "UpdatedAt")
            VALUES
                (910001, 'USD', 'CRC', 500, '2000-01-01', FALSE, NOW(), NOW()),
                (910002, 'USD', 'CRC', 600, '2001-01-01', FALSE, NOW(), NOW()),
                (910003, 'USD', 'CRC', 700, '2002-01-01', FALSE, NOW(), NOW());
            """);
    }

    private async Task RestaurarTiposCambioHistoricosAsync()
    {
        await using var context = _database.CrearContexto();
        await context.Database.ExecuteSqlRawAsync(
            "DELETE FROM \"TiposCambio\" WHERE \"Id\" BETWEEN {0} AND {1}",
            910001,
            910003);
    }

    private async Task<ProveedorDto> CrearProveedorAsync(string nombre)
    {
        await using var context = _database.CrearContexto();
        return await new CrearProveedorService(new ProveedorRepository(context))
            .CrearAsync(new CrearProveedorRequest(nombre));
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
                    services.AddDbContext<LicitacionesDbContext>(options =>
                        options.UseNpgsql(_database.ConnectionString));
                    services.AddDataProtection().UseEphemeralDataProtectionProvider();
                });
            });
    }
}
