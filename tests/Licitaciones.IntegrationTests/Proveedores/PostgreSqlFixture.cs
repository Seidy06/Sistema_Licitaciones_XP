using Licitaciones.Infrastructure.Persistence;
using Licitaciones.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;

namespace Licitaciones.IntegrationTests.Proveedores;

public sealed class PostgreSqlFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer? _container;

    public PostgreSqlFixture()
    {
        var configuredConnectionString =
            Environment.GetEnvironmentVariable("LICITACIONES_INTEGRATION_CONNECTION_STRING");

        if (!string.IsNullOrWhiteSpace(configuredConnectionString))
        {
            ConnectionString = configuredConnectionString;
            return;
        }

        _container = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("licitaciones_db")
            .WithUsername("licitaciones_user")
            .WithPassword("licitaciones_password")
            .Build();
    }

    public string ConnectionString { get; private set; } = string.Empty;

    public LicitacionesDbContext CrearContexto(IClock? clock = null)
    {
        var options = new DbContextOptionsBuilder<LicitacionesDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new LicitacionesDbContext(options, clock);
    }

    public async Task InitializeAsync()
    {
        if (_container is not null)
        {
            await _container.StartAsync();
            ConnectionString = _container.GetConnectionString();
        }

        await using var context = CrearContexto();
        await context.Database.MigrateAsync();
    }

    public Task DisposeAsync() => _container?.DisposeAsync().AsTask() ?? Task.CompletedTask;
}
