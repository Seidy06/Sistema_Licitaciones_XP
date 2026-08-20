using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Proveedores;
using Licitaciones.IntegrationTests.Common;
using Licitaciones.IntegrationTests.Proveedores;

using Microsoft.EntityFrameworkCore;

using Ofertas = Licitaciones.Domain.Ofertas;

using static Licitaciones.IntegrationTests.Common.LicitacionTestHelper;

namespace Licitaciones.IntegrationTests.Hu13;

public sealed class ConsultarLicitacionPersistenceTests : IClassFixture<PostgreSqlFixture>
{
    private static readonly DateTimeOffset Ahora =
        new(2026, 8, 19, 12, 0, 0, TimeSpan.Zero);

    private readonly PostgreSqlFixture _database;

    public ConsultarLicitacionPersistenceTests(PostgreSqlFixture database) =>
        _database = database;

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task PostgreSql_ListarPorEstadoPublicada_DebeRetornarSoloPublicadas()
    {
        var publicada = PublicarLicitacion($"PUB-{Guid.NewGuid():N}", Ahora.AddDays(5));
        var borrador = Licitacion.Crear(
            $"BOR-{Guid.NewGuid():N}", "Borrador", 1000m, Ahora.AddDays(5));

        await using (var context = _database.CrearContexto())
        {
            context.Licitaciones.AddRange(publicada, borrador);
            await context.SaveChangesAsync();
        }

        await using var queryContext = _database.CrearContexto();
        var results = await queryContext.Licitaciones
            .AsNoTracking()
            .Where(l => l.Id == publicada.Id
                && l.Estado == EstadoLicitacion.Publicada
                && l.DeletedAt == null)
            .ToListAsync();

        Assert.Single(results);
        Assert.Equal(publicada.Id, results[0].Id);
    }

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task PostgreSql_ListarPorEstadoCerrada_DebeIncluirCierreFuncional()
    {
        var publicadaVencida = PublicarLicitacion(
            $"VENC-{Guid.NewGuid():N}",
            Ahora.AddTicks(-1));

        await using (var context = _database.CrearContexto())
        {
            context.Licitaciones.Add(publicadaVencida);
            await context.SaveChangesAsync();
        }

        await using var queryContext = _database.CrearContexto();
        var results = await queryContext.Licitaciones
            .AsNoTracking()
            .Where(l => l.Id == publicadaVencida.Id
                && l.Estado == EstadoLicitacion.Publicada
                && l.FechaCierre <= Ahora
                && l.DeletedAt == null)
            .ToListAsync();

        Assert.Single(results);
        Assert.Equal(publicadaVencida.Id, results[0].Id);
    }

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task PostgreSql_DetalleConOferta_DebePersistirMontoOferta()
    {
        var licitacion = PublicarLicitacion(
            $"DET-{Guid.NewGuid():N}", Ahora.AddDays(5));
        var proveedor = Proveedor.Crear($"Proveedor {Guid.NewGuid():N}");

        await using (var context = _database.CrearContexto())
        {
            context.Proveedores.Add(proveedor);
            context.Licitaciones.Add(licitacion);
            await context.SaveChangesAsync();

            context.Ofertas.Add(Ofertas.Oferta.Crear(
                licitacion.Id, proveedor.Id, 8_000m, new FixedClock(Ahora)));
            await context.SaveChangesAsync();
        }

        await using var queryContext = _database.CrearContexto();
        var montoMinimo = await queryContext.Ofertas
            .Where(o => o.LicitacionId == licitacion.Id)
            .MinAsync(o => (decimal?)o.Monto);

        Assert.NotNull(montoMinimo);
        Assert.Equal(8_000m, montoMinimo);
    }

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task PostgreSql_DetalleConOferta_DebePersistirMontoCorrecto()
    {
        var licitacion = PublicarLicitacion(
            $"MULT-{Guid.NewGuid():N}", Ahora.AddDays(5));
        var proveedor1 = Proveedor.Crear($"Proveedor 1 {Guid.NewGuid():N}");
        var proveedor2 = Proveedor.Crear($"Proveedor 2 {Guid.NewGuid():N}");

        await using (var context = _database.CrearContexto())
        {
            context.Proveedores.AddRange(proveedor1, proveedor2);
            context.Licitaciones.Add(licitacion);
            await context.SaveChangesAsync();

            context.Ofertas.Add(Ofertas.Oferta.Crear(
                licitacion.Id, proveedor1.Id, 15_000m, new FixedClock(Ahora)));
            context.Ofertas.Add(Ofertas.Oferta.Crear(
                licitacion.Id, proveedor2.Id, 12_000m, new FixedClock(Ahora)));
            await context.SaveChangesAsync();
        }

        await using var queryContext = _database.CrearContexto();
        var montoMinimo = await queryContext.Ofertas
            .Where(o => o.LicitacionId == licitacion.Id)
            .MinAsync(o => (decimal?)o.Monto);

        Assert.Equal(12_000m, montoMinimo);
    }
}
