using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Application.Proveedores.Eliminar;
using Licitaciones.Domain.Common;
using Licitaciones.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Licitaciones.IntegrationTests.Proveedores;

public sealed class ProveedorBajaPersistenceTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _database;

    public ProveedorBajaPersistenceTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-08")]
    public async Task DarDeBaja_DebeOcultarloSinEliminarFilaNiInformacionHistorica()
    {
        var creado = await CrearProveedorAsync($"Histórico {Guid.NewGuid():N}");
        var instante = new DateTimeOffset(2026, 8, 14, 20, 0, 0, TimeSpan.Zero);

        await using (var context = _database.CrearContexto(new FixedClock(instante)))
        {
            var service = new DarBajaProveedorService(
                new ProveedorRepository(context),
                new FixedClock(instante));
            await service.DarDeBajaAsync(creado.Id);
        }

        await using var verificationContext = _database.CrearContexto();
        Assert.False(await verificationContext.Proveedores
            .AnyAsync(proveedor => proveedor.Id == creado.Id));

        var historico = await verificationContext.Proveedores
            .IgnoreQueryFilters()
            .SingleAsync(proveedor => proveedor.Id == creado.Id);

        Assert.Equal(instante, historico.DeletedAt);
        Assert.Equal(creado.Nombre, historico.Nombre);
        Assert.Equal(creado.NombreNormalizado, historico.NombreNormalizado);
        Assert.Equal(creado.CreatedAt, historico.CreatedAt);
        Assert.Equal(1, await verificationContext.Proveedores
            .IgnoreQueryFilters()
            .CountAsync(proveedor => proveedor.Id == creado.Id));
    }

    [Fact]
    [Trait("HU", "HU-08")]
    public void Modelo_DebeConfigurarFiltroGlobalParaProveedoresEliminados()
    {
        using var context = _database.CrearContexto();
        var entityType = context.Model.FindEntityType(
            typeof(Licitaciones.Domain.Proveedores.Proveedor));

        var filter = entityType?.GetQueryFilter();

        Assert.NotNull(filter);
        Assert.Contains("DeletedAt", filter.ToString(), StringComparison.Ordinal);
    }

    private async Task<Licitaciones.Application.Proveedores.ProveedorDto> CrearProveedorAsync(
        string nombre)
    {
        await using var context = _database.CrearContexto();
        return await new CrearProveedorService(new ProveedorRepository(context))
            .CrearAsync(new CrearProveedorRequest(nombre));
    }

    private sealed class FixedClock : IClock
    {
        private readonly DateTimeOffset _utcNow;

        public FixedClock(DateTimeOffset utcNow) => _utcNow = utcNow;

        public DateTimeOffset UtcNow() => _utcNow;
    }
}
