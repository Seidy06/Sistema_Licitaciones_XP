using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Application.Proveedores.Editar;
using Licitaciones.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Licitaciones.IntegrationTests.Proveedores;

public sealed class ProveedorEdicionPersistenceTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _database;

    public ProveedorEdicionPersistenceTests(PostgreSqlFixture database) =>
        _database = database;

    [Fact]
    [Trait("HU", "HU-07")]
    public async Task EditarProveedor_DebePersistirNombreNormalizadoDeHu06()
    {
        var creado = await CrearProveedorAsync($"Original {Guid.NewGuid():N}");

        await using (var context = _database.CrearContexto())
        {
            var service = new EditarProveedorService(new ProveedorRepository(context));
            await service.EditarAsync(
                creado.Id,
                new EditarProveedorRequest($"  Cafe\u0301    Central {creado.Id:N}", creado.Version));
        }

        await using var verificationContext = _database.CrearContexto();
        var guardado = await verificationContext.Proveedores
            .AsNoTracking()
            .SingleAsync(proveedor => proveedor.Id == creado.Id);

        Assert.Equal($"Café Central {creado.Id:N}", guardado.Nombre);
        Assert.Equal($"CAFÉ CENTRAL {creado.Id:N}".ToUpperInvariant(), guardado.NombreNormalizado);
        Assert.True(guardado.Version > creado.Version);
    }

    [Fact]
    [Trait("HU", "HU-07")]
    public async Task EditarProveedor_ConVersionXminDesactualizada_DebeProducirConflictoControlado()
    {
        var creado = await CrearProveedorAsync($"Concurrente {Guid.NewGuid():N}");

        await using (var firstContext = _database.CrearContexto())
        {
            var firstService = new EditarProveedorService(new ProveedorRepository(firstContext));
            await firstService.EditarAsync(
                creado.Id,
                new EditarProveedorRequest($"Primer cambio {creado.Id:N}", creado.Version));
        }

        await using var staleContext = _database.CrearContexto();
        var staleService = new EditarProveedorService(new ProveedorRepository(staleContext));

        await Assert.ThrowsAsync<ProveedorConcurrenciaException>(() =>
            staleService.EditarAsync(
                creado.Id,
                new EditarProveedorRequest($"Cambio obsoleto {creado.Id:N}", creado.Version)));
    }

    private async Task<Licitaciones.Application.Proveedores.ProveedorDto> CrearProveedorAsync(
        string nombre)
    {
        await using var context = _database.CrearContexto();
        var service = new CrearProveedorService(new ProveedorRepository(context));
        return await service.CrearAsync(new CrearProveedorRequest(nombre));
    }
}
