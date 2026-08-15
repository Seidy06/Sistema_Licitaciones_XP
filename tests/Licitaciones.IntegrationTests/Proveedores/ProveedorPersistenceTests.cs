using Licitaciones.Application.Proveedores;
using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Domain.Proveedores;
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
    [Trait("HU", "HU-06-Auditoria")]
    public async Task CrearProveedor_DebeTratarUnicodeDescompuestoComoNombreEquivalente()
    {
        var sufijo = Guid.NewGuid().ToString("N");

        await using (var primerContexto = _database.CrearContexto())
        {
            var service = new CrearProveedorService(new ProveedorRepository(primerContexto));
            var creado = await service.CrearAsync(
                new CrearProveedorRequest($"Cafe\u0301 Central {sufijo}"));

            Assert.Equal($"Café Central {sufijo}", creado.Nombre);
            Assert.Equal($"CAFÉ CENTRAL {sufijo.ToUpperInvariant()}", creado.NombreNormalizado);
        }

        await using var segundoContexto = _database.CrearContexto();
        var segundoService = new CrearProveedorService(new ProveedorRepository(segundoContexto));

        await Assert.ThrowsAsync<ProveedorDuplicadoException>(() =>
            segundoService.CrearAsync(
                new CrearProveedorRequest($" CAFÉ   CENTRAL {sufijo} ")));
    }

    [Fact]
    [Trait("HU", "HU-06-Auditoria")]
    public async Task CrearProveedor_Concurrentemente_DebeCrearUnoYRechazarElDuplicado()
    {
        var sufijo = Guid.NewGuid().ToString("N");
        var barrera = new BarreraDosConsultas();

        await using var primerContexto = _database.CrearContexto();
        await using var segundoContexto = _database.CrearContexto();
        var primerService = CrearServicioSincronizado(primerContexto, barrera);
        var segundoService = CrearServicioSincronizado(segundoContexto, barrera);

        var resultados = await Task.WhenAll(
            CapturarExcepcionAsync(() => primerService.CrearAsync(
                new CrearProveedorRequest($"Empresa   Concurrente {sufijo}"))),
            CapturarExcepcionAsync(() => segundoService.CrearAsync(
                new CrearProveedorRequest($" empresa concurrente {sufijo} "))));

        Assert.Single(resultados, resultado => resultado is null);
        var rechazo = Assert.Single(resultados, resultado => resultado is not null);
        Assert.IsType<ProveedorDuplicadoException>(rechazo);
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

    private static CrearProveedorService CrearServicioSincronizado(
        LicitacionesDbContext context,
        BarreraDosConsultas barrera)
    {
        IProveedorRepository repository = new ProveedorRepository(context);
        return new CrearProveedorService(
            new RepositorioConConsultaSincronizada(repository, barrera));
    }

    private static async Task<Exception?> CapturarExcepcionAsync(
        Func<Task<ProveedorDto>> operacion)
    {
        try
        {
            await operacion();
            return null;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }

    private sealed class RepositorioConConsultaSincronizada : IProveedorRepository
    {
        private readonly IProveedorRepository _inner;
        private readonly BarreraDosConsultas _barrera;

        public RepositorioConConsultaSincronizada(
            IProveedorRepository inner,
            BarreraDosConsultas barrera)
        {
            _inner = inner;
            _barrera = barrera;
        }

        public async Task<bool> ExisteNombreNormalizadoAsync(
            string nombreNormalizado,
            CancellationToken cancellationToken = default)
        {
            var existe = await _inner.ExisteNombreNormalizadoAsync(
                nombreNormalizado,
                cancellationToken);
            await _barrera.EsperarAmbasConsultasAsync(cancellationToken);
            return existe;
        }

        public Task AgregarAsync(
            Proveedor proveedor,
            CancellationToken cancellationToken = default)
        {
            return _inner.AgregarAsync(proveedor, cancellationToken);
        }
    }

    private sealed class BarreraDosConsultas
    {
        private readonly TaskCompletionSource _ambasConsultaron =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int _cantidadConsultas;

        public async Task EsperarAmbasConsultasAsync(CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref _cantidadConsultas) == 2)
            {
                _ambasConsultaron.TrySetResult();
            }

            await _ambasConsultaron.Task.WaitAsync(cancellationToken);
        }
    }
}
