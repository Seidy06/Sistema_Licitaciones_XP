using Licitaciones.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Licitaciones.IntegrationTests.Proveedores;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    public string ConnectionString { get; } =
        Environment.GetEnvironmentVariable("LICITACIONES_INTEGRATION_CONNECTION_STRING")
        ?? CrearConnectionStringLocal();

    public LicitacionesDbContext CrearContexto()
    {
        var options = new DbContextOptionsBuilder<LicitacionesDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new LicitacionesDbContext(options);
    }

    public async Task InitializeAsync()
    {
        await using var context = CrearContexto();
        await context.Database.MigrateAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    private static string CrearConnectionStringLocal()
    {
        var configuration = CargarDotEnv();
        return new NpgsqlConnectionStringBuilder
        {
            Host = "localhost",
            Port = LeerEntero(configuration, "POSTGRES_PORT", 5432),
            Database = Leer(configuration, "POSTGRES_DB", "licitaciones_db"),
            Username = Leer(configuration, "POSTGRES_USER", "licitaciones_user"),
            Password = Leer(configuration, "POSTGRES_PASSWORD", "licitaciones_password")
        }.ConnectionString;
    }

    private static Dictionary<string, string> CargarDotEnv()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var path = Path.Combine(directory.FullName, ".env");
            if (File.Exists(path))
            {
                return File.ReadLines(path)
                    .Where(line => !string.IsNullOrWhiteSpace(line) && !line.TrimStart().StartsWith('#'))
                    .Select(line => line.Split('=', 2))
                    .Where(parts => parts.Length == 2)
                    .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());
            }

            directory = directory.Parent;
        }

        return [];
    }

    private static string Leer(
        IReadOnlyDictionary<string, string> configuration,
        string key,
        string defaultValue)
    {
        return configuration.GetValueOrDefault(key, defaultValue);
    }

    private static int LeerEntero(
        IReadOnlyDictionary<string, string> configuration,
        string key,
        int defaultValue)
    {
        return configuration.TryGetValue(key, out var value) && int.TryParse(value, out var result)
            ? result
            : defaultValue;
    }
}
