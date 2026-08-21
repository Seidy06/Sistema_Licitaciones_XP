using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Domain.Proveedores;
using Licitaciones.Infrastructure.Persistence;

namespace Licitaciones.IntegrationTests.Proveedores;

[Collection(PostgreSqlCollection.Name)]
public sealed class ProveedorConsultaPersistenceTests
{
    private readonly PostgreSqlFixture _database;

    public ProveedorConsultaPersistenceTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-09")]
    public async Task ListarAsync_DebeFiltrarNombreSinDistinguirMayusculasYPaginar()
    {
        var sufijo = Guid.NewGuid().ToString("N");
        await GuardarAsync(
            Proveedor.Crear($"Alfa {sufijo}"),
            Proveedor.Crear($"ALFA Dos {sufijo}"),
            Proveedor.Crear($"Beta {sufijo}"));
        await using var context = _database.CrearContexto();
        var repository = new ProveedorRepository(context);

        var pagina = await repository.ListarAsync(new ConsultarProveedoresRequest(
            pagina: 2, tamanoPagina: 1, nombre: $"aLfA", ordenarPor: ProveedorOrden.Nombre));

        Assert.Equal(2, pagina.Total);
        Assert.Single(pagina.Items);
        Assert.Contains(sufijo, pagina.Items[0].Nombre);
    }

    [Fact]
    [Trait("HU", "HU-09")]
    public async Task ListarAsync_DebeOrdenarPorNombreYPorFechaCreacion()
    {
        var sufijo = Guid.NewGuid().ToString("N");
        await GuardarAsync(
            Proveedor.Crear($"Zulu {sufijo}"),
            Proveedor.Crear($"Alfa {sufijo}"));
        await using var context = _database.CrearContexto();
        var repository = new ProveedorRepository(context);

        var porNombre = await repository.ListarAsync(new ConsultarProveedoresRequest(
            1, 10, sufijo, ProveedorOrden.Nombre));
        var porFecha = await repository.ListarAsync(new ConsultarProveedoresRequest(
            1, 10, sufijo, ProveedorOrden.FechaCreacion, descendente: true));

        Assert.StartsWith("Alfa", porNombre.Items[0].Nombre);
        Assert.True(porFecha.Items[0].CreatedAt >= porFecha.Items[1].CreatedAt);
    }

    private async Task GuardarAsync(params Proveedor[] proveedores)
    {
        await using var context = _database.CrearContexto();
        context.Proveedores.AddRange(proveedores);
        await context.SaveChangesAsync();
    }
}
