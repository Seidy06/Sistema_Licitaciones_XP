using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Licitaciones.IntegrationTests.Proveedores;

public sealed class ProveedorPersistenceTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _database;

    public ProveedorPersistenceTests(PostgreSqlFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task CrearProveedor_DebePersistirEnPostgreSQL()
    {
        var nombre = $"Empresa Central {Guid.NewGuid():N}";
        Guid proveedorId;

        await using (var context = _database.CrearContexto())
        {
            var service = new CrearProveedorService(new ProveedorRepository(context));
            var resultado = await service.CrearAsync(new CrearProveedorRequest(nombre));
            proveedorId = resultado.Id;
        }

        await using var contextoRecuperado = _database.CrearContexto();
        var proveedor = await contextoRecuperado.Proveedores
            .AsNoTracking()
            .SingleAsync(item => item.Id == proveedorId);

        Assert.Equal(nombre, proveedor.Nombre);
        Assert.Equal(nombre.ToUpperInvariant(), proveedor.NombreNormalizado);
        Assert.NotEqual(default, proveedor.CreatedAt);
        Assert.Equal(proveedor.CreatedAt, proveedor.UpdatedAt);
    }

    [Fact]
    public async Task CrearProveedor_DebeRechazarNombreNormalizadoDuplicado()
    {
        var sufijo = Guid.NewGuid().ToString("N");

        await using (var primerContexto = _database.CrearContexto())
        {
            var service = new CrearProveedorService(new ProveedorRepository(primerContexto));
            await service.CrearAsync(new CrearProveedorRequest($"Empresa   Central {sufijo}"));
        }

        await using var segundoContexto = _database.CrearContexto();
        var segundoService = new CrearProveedorService(new ProveedorRepository(segundoContexto));

        await Assert.ThrowsAsync<ProveedorDuplicadoException>(() =>
            segundoService.CrearAsync(
                new CrearProveedorRequest($"  empresa central {sufijo}  ")));
    }

    [Fact]
    public async Task Migracion_DebeCrearIndiceUnico()
    {
        await using var connection = new NpgsqlConnection(_database.ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT i.indisunique
            FROM pg_class tabla
            JOIN pg_index i ON i.indrelid = tabla.oid
            JOIN pg_class indice ON indice.oid = i.indexrelid
            WHERE tabla.relname = 'Proveedores'
              AND indice.relname = 'UX_Proveedores_NombreNormalizado';
            """;

        var esUnico = await command.ExecuteScalarAsync();

        Assert.NotNull(esUnico);
        Assert.True(Convert.ToBoolean(esUnico));
    }
}
