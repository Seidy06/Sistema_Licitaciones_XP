using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;
using Licitaciones.Domain.Proveedores;
using Licitaciones.IntegrationTests.Proveedores;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Licitaciones.IntegrationTests.Persistence;

public sealed class InitialModelPersistenceTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _database;

    public InitialModelPersistenceTests(PostgreSqlFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task Migracion_DebeSembrarCatalogosIniciales()
    {
        await using var context = _database.CrearContexto();

        Assert.Equal(5, await context.EstadosLicitacion.CountAsync());
        Assert.True(await context.NivelesAprobacion.CountAsync() >= 3);
        Assert.Single(await context.TiposCambio.Where(x => x.Activo).ToListAsync());
    }

    [Fact]
    public async Task Migraciones_DebenEstarAplicadasEnPostgreSqlReal()
    {
        await using var context = _database.CrearContexto();

        var aplicadas = (await context.Database.GetAppliedMigrationsAsync()).ToArray();
        var pendientes = (await context.Database.GetPendingMigrationsAsync()).ToArray();

        Assert.Contains("20260810005236_CreateProviders", aplicadas);
        Assert.Contains("20260815003821_AddProveedorSoftDelete", aplicadas);
        Assert.Empty(pendientes);
    }

    [Fact]
    public async Task SaveChanges_DebeAsignarTimestampsConRelojInyectado()
    {
        var instante = new DateTimeOffset(2026, 8, 12, 15, 30, 0, TimeSpan.Zero);
        await using var context = _database.CrearContexto(new FixedClock(instante));
        var licitacion = Licitacion.Crear("CLOCK-" + Guid.NewGuid().ToString("N"), "Prueba de reloj", 100m, instante.AddDays(1));

        context.Licitaciones.Add(licitacion);
        await context.SaveChangesAsync();

        Assert.Equal(instante, licitacion.CreatedAt);
        Assert.Equal(instante, licitacion.UpdatedAt);
    }

    [Fact]
    public async Task BaseDatos_DebeRechazarNivelesDeAprobacionTraslapados()
    {
        await using var context = _database.CrearContexto();

        await Assert.ThrowsAsync<PostgresException>(() =>
            context.Database.ExecuteSqlRawAsync("""
                INSERT INTO "NivelesAprobacion"
                    ("Id", "Nombre", "MontoMinimo", "MontoMaximo", "CreatedAt", "UpdatedAt")
                VALUES
                    (99, 'Traslapado', 500000, 1500000, NOW(), NOW());
                """));
    }

    [Fact]
    public async Task BaseDatos_DebeRechazarOfertaConMontoNoPositivo()
    {
        await using var context = _database.CrearContexto();
        var ahora = DateTimeOffset.UtcNow;
        var proveedor = Proveedor.Crear("Proveedor constraint " + Guid.NewGuid().ToString("N"));
        var licitacion = Licitacion.Crear(
            "CHECK-" + Guid.NewGuid().ToString("N"),
            "Prueba de restricción",
            100m,
            ahora.AddDays(1));
        var oferta = Oferta.Crear(licitacion.Id, proveedor.Id, 1m, new FixedClock(ahora));

        context.AddRange(proveedor, licitacion, oferta);
        context.Entry(oferta).Property(x => x.Monto).CurrentValue = 0m;

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    private sealed class FixedClock : IClock
    {
        private readonly DateTimeOffset _value;

        public FixedClock(DateTimeOffset value) => _value = value;

        public DateTimeOffset UtcNow() => _value;
    }
}
