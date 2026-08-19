using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.IntegrationTests.Proveedores;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Licitaciones.IntegrationTests.Hu11;

public sealed class PublicarLicitacionPersistenceTests : IClassFixture<PostgreSqlFixture>
{
    private static readonly DateTimeOffset Ahora =
        new(2026, 8, 18, 15, 0, 0, TimeSpan.Zero);

    private readonly PostgreSqlFixture _database;

    public PublicarLicitacionPersistenceTests(PostgreSqlFixture database) =>
        _database = database;

    [Fact]
    [Trait("HU", "HU-11")]
    public async Task PostgreSql_PublicarBorrador_DebePersistirEstadoYTransicion()
    {
        var licitacion = Licitacion.Crear(
            $"HU11-{Guid.NewGuid():N}",
            "Compra para prueba de publicación",
            1000m,
            Ahora.AddDays(1));

        await using (var context = _database.CrearContexto(new FixedClock(Ahora)))
        {
            context.Licitaciones.Add(licitacion);
            await context.SaveChangesAsync();

            licitacion.Publicar(new FixedClock(Ahora));
            await context.SaveChangesAsync();
        }

        await using var verificationContext = _database.CrearContexto();
        var persisted = await verificationContext.Licitaciones
            .AsNoTracking()
            .SingleAsync(item => item.Id == licitacion.Id);

        Assert.Equal(EstadoLicitacion.Publicada, persisted.Estado);

        await using var connection = new NpgsqlConnection(_database.ConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(*)
            FROM licitacion_transiciones
            WHERE licitacion_id = @licitacionId
              AND estado_anterior = @borrador
              AND estado_nuevo = @publicada
            """;
        command.Parameters.AddWithValue("licitacionId", licitacion.Id);
        command.Parameters.AddWithValue("borrador", (int)EstadoLicitacion.Borrador);
        command.Parameters.AddWithValue("publicada", (int)EstadoLicitacion.Publicada);

        Assert.Equal(1L, (long)(await command.ExecuteScalarAsync())!);
    }

    private sealed class FixedClock : IClock
    {
        private readonly DateTimeOffset _value;

        public FixedClock(DateTimeOffset value) => _value = value;

        public DateTimeOffset UtcNow() => _value;
    }
}
