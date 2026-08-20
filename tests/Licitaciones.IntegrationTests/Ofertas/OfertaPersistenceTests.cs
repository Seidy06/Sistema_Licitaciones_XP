using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;
using Licitaciones.Domain.Proveedores;
using Licitaciones.IntegrationTests.Proveedores;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Licitaciones.IntegrationTests.Hu14;

public sealed class OfertaPersistenceTests : IClassFixture<PostgreSqlFixture>
{
    private static readonly DateTimeOffset Ahora =
        new(2026, 8, 19, 15, 0, 0, TimeSpan.Zero);

    private readonly PostgreSqlFixture _database;

    public OfertaPersistenceTests(PostgreSqlFixture database) =>
        _database = database;

    [Fact]
    [Trait("HU", "HU-14")]
    public async Task PostgreSql_ConMismosLicitacionYProveedor_DebeRechazarDuplicado()
    {
        var licitacion = Licitacion.Crear(
            $"HU14-DUP-{Guid.NewGuid():N}",
            "Compra para prueba duplicado",
            10_000m,
            Ahora.AddDays(10));
        EstablecerEstado(licitacion, EstadoLicitacion.Publicada);

        var proveedor = Proveedor.Crear($"DUP-{Guid.NewGuid():N}");

        await using (var context = _database.CrearContexto(new FixedClock(Ahora)))
        {
            context.Licitaciones.Add(licitacion);
            context.Proveedores.Add(proveedor);
            await context.SaveChangesAsync();
        }

        await using (var firstContext = _database.CrearContexto(new FixedClock(Ahora)))
        {
            firstContext.Ofertas.Add(Oferta.Crear(
                licitacion.Id, proveedor.Id, 500m, new FixedClock(Ahora)));
            await firstContext.SaveChangesAsync();
        }

        await using var secondContext = _database.CrearContexto(new FixedClock(Ahora));
        secondContext.Ofertas.Add(Oferta.Crear(
            licitacion.Id, proveedor.Id, 600m, new FixedClock(Ahora)));

        await Assert.ThrowsAsync<DbUpdateException>(
            () => secondContext.SaveChangesAsync());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [Trait("HU", "HU-14")]
    public async Task PostgreSql_ConMontoNoPositivo_DebeAplicarCheck(decimal monto)
    {
        var licitacion = Licitacion.Crear(
            $"HU14-CHECK-{Guid.NewGuid():N}",
            "Compra para prueba check monto",
            10_000m,
            Ahora.AddDays(10));

        var proveedor = Proveedor.Crear($"CHECK-{Guid.NewGuid():N}");

        await using (var context = _database.CrearContexto(new FixedClock(Ahora)))
        {
            context.Licitaciones.Add(licitacion);
            context.Proveedores.Add(proveedor);
            await context.SaveChangesAsync();
        }

        await using var testContext = _database.CrearContexto(new FixedClock(Ahora));
        var oferta = Oferta.Crear(
            licitacion.Id, proveedor.Id, 1m, new FixedClock(Ahora));
        testContext.Ofertas.Add(oferta);
        testContext.Entry(oferta).Property(x => x.Monto).CurrentValue = monto;

        await Assert.ThrowsAsync<DbUpdateException>(
            () => testContext.SaveChangesAsync());
    }

    [Fact]
    [Trait("HU", "HU-14")]
    public async Task PostgreSql_ConMontoMayorAlPresupuesto_DebePermitirEnBaseDeDatos()
    {
        var presupuesto = 10_000m;
        var licitacion = Licitacion.Crear(
            $"HU14-PRESUP-{Guid.NewGuid():N}",
            "Compra con presupuesto",
            presupuesto,
            Ahora.AddDays(10));

        var proveedor = Proveedor.Crear($"PRESUP-{Guid.NewGuid():N}");

        await using (var context = _database.CrearContexto(new FixedClock(Ahora)))
        {
            context.Licitaciones.Add(licitacion);
            context.Proveedores.Add(proveedor);
            await context.SaveChangesAsync();
        }

        await using var testContext = _database.CrearContexto(new FixedClock(Ahora));
        testContext.Ofertas.Add(Oferta.Crear(
            licitacion.Id, proveedor.Id, presupuesto + 1m, new FixedClock(Ahora)));

        await testContext.SaveChangesAsync();
    }

    [Fact]
    [Trait("HU", "HU-14")]
    public async Task PostgreSql_ConMontoIgualAlPresupuesto_DebePermitirEnBaseDeDatos()
    {
        var presupuesto = 10_000m;
        var licitacion = Licitacion.Crear(
            $"HU14-PRESUP2-{Guid.NewGuid():N}",
            "Compra con monto exacto",
            presupuesto,
            Ahora.AddDays(10));

        var proveedor = Proveedor.Crear($"PRESUP2-{Guid.NewGuid():N}");

        await using (var context = _database.CrearContexto(new FixedClock(Ahora)))
        {
            context.Licitaciones.Add(licitacion);
            context.Proveedores.Add(proveedor);
            await context.SaveChangesAsync();
        }

        await using var testContext = _database.CrearContexto(new FixedClock(Ahora));
        testContext.Ofertas.Add(Oferta.Crear(
            licitacion.Id, proveedor.Id, presupuesto, new FixedClock(Ahora)));

        await testContext.SaveChangesAsync();
    }

    [Fact]
    [Trait("HU", "HU-14")]
    public async Task PostgreSql_OfertaConProveedorNoExistente_DebeRechazarFK()
    {
        var licitacion = Licitacion.Crear(
            $"HU14-FK-{Guid.NewGuid():N}",
            "Compra para prueba FK",
            10_000m,
            Ahora.AddDays(10));

        await using (var context = _database.CrearContexto(new FixedClock(Ahora)))
        {
            context.Licitaciones.Add(licitacion);
            await context.SaveChangesAsync();
        }

        await using var testContext = _database.CrearContexto(new FixedClock(Ahora));
        testContext.Ofertas.Add(Oferta.Crear(
            licitacion.Id, Guid.NewGuid(), 500m, new FixedClock(Ahora)));

        await Assert.ThrowsAsync<DbUpdateException>(
            () => testContext.SaveChangesAsync());
    }

    [Fact]
    [Trait("HU", "HU-14")]
    public async Task PostgreSql_OfertaConLicitacionNoExistente_DebeRechazarFK()
    {
        var proveedor = Proveedor.Crear($"FK-LIC-{Guid.NewGuid():N}");

        await using (var context = _database.CrearContexto(new FixedClock(Ahora)))
        {
            context.Proveedores.Add(proveedor);
            await context.SaveChangesAsync();
        }

        await using var testContext = _database.CrearContexto(new FixedClock(Ahora));
        testContext.Ofertas.Add(Oferta.Crear(
            Guid.NewGuid(), proveedor.Id, 500m, new FixedClock(Ahora)));

        await Assert.ThrowsAsync<DbUpdateException>(
            () => testContext.SaveChangesAsync());
    }

    private static void EstablecerEstado(
        Licitacion licitacion, EstadoLicitacion estado)
    {
        typeof(Licitacion)
            .GetProperty(nameof(Licitacion.Estado),
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public)!
            .SetValue(licitacion, estado);
    }

    private sealed class FixedClock : IClock
    {
        private readonly DateTimeOffset _value;
        public FixedClock(DateTimeOffset value) => _value = value;
        public DateTimeOffset UtcNow() => _value;
    }
}
