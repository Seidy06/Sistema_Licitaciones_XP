using Licitaciones.Domain.Licitaciones;
using Licitaciones.IntegrationTests.Proveedores;

using Microsoft.EntityFrameworkCore;

namespace Licitaciones.IntegrationTests.Hu10;

public sealed class CrearLicitacionPersistenceTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _database;

    public CrearLicitacionPersistenceTests(PostgreSqlFixture database) =>
        _database = database;

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [Trait("HU", "HU-10")]
    public async Task PostgreSql_ConPresupuestoNoPositivo_DebeAplicarCheck(decimal presupuesto)
    {
        await using var context = _database.CrearContexto();
        var licitacion = NuevaLicitacion($"CHECK-{Guid.NewGuid():N}");
        context.Licitaciones.Add(licitacion);
        context.Entry(licitacion).Property(x => x.Presupuesto).CurrentValue = presupuesto;

        await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
    }

    [Fact]
    [Trait("HU", "HU-10")]
    public async Task PostgreSql_ConCodigosEquivalentes_DebeRechazarDuplicado()
    {
        var codigo = $"hu10-{Guid.NewGuid():N}";
        await using (var firstContext = _database.CrearContexto())
        {
            firstContext.Licitaciones.Add(NuevaLicitacion(codigo));
            await firstContext.SaveChangesAsync();
        }

        await using var secondContext = _database.CrearContexto();
        secondContext.Licitaciones.Add(NuevaLicitacion($"  {codigo.ToUpperInvariant()}  "));

        await Assert.ThrowsAsync<DbUpdateException>(() => secondContext.SaveChangesAsync());
    }

    [Fact]
    [Trait("HU", "HU-10")]
    public async Task Persistir_NuevaLicitacion_DebeGuardarEstadoBorrador()
    {
        var licitacion = NuevaLicitacion($"BORRADOR-{Guid.NewGuid():N}");
        await using (var context = _database.CrearContexto())
        {
            context.Licitaciones.Add(licitacion);
            await context.SaveChangesAsync();
        }

        await using var verificationContext = _database.CrearContexto();
        var persisted = await verificationContext.Licitaciones
            .AsNoTracking()
            .SingleAsync(item => item.Id == licitacion.Id);

        Assert.Equal(EstadoLicitacion.Borrador, persisted.Estado);
    }

    private static Licitacion NuevaLicitacion(string codigo) => Licitacion.Crear(
        codigo,
        "Compra para pruebas HU-10",
        1000m,
        DateTimeOffset.UtcNow.AddDays(1));
}
