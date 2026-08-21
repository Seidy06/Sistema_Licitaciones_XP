using Licitaciones.IntegrationTests.Proveedores;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Licitaciones.IntegrationTests.Hu18;

[Collection(PostgreSqlCollection.Name)]
public sealed class NivelAprobacionOverlapPersistenceTests
{
    private readonly PostgreSqlFixture _database;

    public NivelAprobacionOverlapPersistenceTests(PostgreSqlFixture database) =>
        _database = database;

    [Fact]
    [Trait("HU", "HU-18")]
    public async Task Guardar_SegundoNivelConRangoTraslapado_DebeSerRechazadoPorPostgreSql()
    {
        await using var context = _database.CrearContexto();
        await using var transaccion = await context.Database.BeginTransactionAsync();
        try
        {
            await context.Database.ExecuteSqlRawAsync("""
                DELETE FROM "NivelesAprobacion";
                """);

            await context.Database.ExecuteSqlRawAsync("""
                INSERT INTO "NivelesAprobacion"
                    ("Id", "Nombre", "MontoMinimo", "MontoMaximo", "CreatedAt", "UpdatedAt")
                VALUES
                    (900001, 'Compras Menores', 20000000, 25000000, NOW(), NOW());
                """);

            var excepcion = await Assert.ThrowsAsync<PostgresException>(() =>
                context.Database.ExecuteSqlRawAsync("""
                    INSERT INTO "NivelesAprobacion"
                        ("Id", "Nombre", "MontoMinimo", "MontoMaximo", "CreatedAt", "UpdatedAt")
                    VALUES
                        (900003, 'Compras Traslapadas', 24000000, 30000000, NOW(), NOW());
                    """));

            Assert.Equal("23P01", excepcion.SqlState);
        }
        finally
        {
            await transaccion.RollbackAsync();
        }
    }

    [Fact]
    [Trait("HU", "HU-18")]
    public async Task Guardar_SegundoRangoAbierto_DebeSerRechazadoPorPostgreSql()
    {
        await using var context = _database.CrearContexto();
        await using var transaccion = await context.Database.BeginTransactionAsync();
        try
        {
            await context.Database.ExecuteSqlRawAsync("""
                DELETE FROM "NivelesAprobacion";
                """);

            await context.Database.ExecuteSqlRawAsync("""
                INSERT INTO "NivelesAprobacion"
                    ("Id", "Nombre", "MontoMinimo", "MontoMaximo", "CreatedAt", "UpdatedAt")
                VALUES
                    (900001, 'Compras Mayores', 40000000, NULL, NOW(), NOW());
                """);

            var excepcion = await Assert.ThrowsAsync<PostgresException>(() =>
                context.Database.ExecuteSqlRawAsync("""
                    INSERT INTO "NivelesAprobacion"
                        ("Id", "Nombre", "MontoMinimo", "MontoMaximo", "CreatedAt", "UpdatedAt")
                    VALUES
                        (900003, 'Junta Directiva Ampliada', 50000000, NULL, NOW(), NOW());
                    """));

            Assert.Equal("23P01", excepcion.SqlState);
        }
        finally
        {
            await transaccion.RollbackAsync();
        }
    }
}
