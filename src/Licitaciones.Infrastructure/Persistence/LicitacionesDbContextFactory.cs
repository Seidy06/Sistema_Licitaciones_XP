using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Licitaciones.Infrastructure.Persistence;

/// <summary>
/// Fábrica de contexto para herramientas de diseño de EF Core (migraciones).
/// </summary>
public sealed class LicitacionesDbContextFactory
    : IDesignTimeDbContextFactory<LicitacionesDbContext>
{
    /// <summary>
    /// Crea una instancia del contexto usando la cadena de conexión de diseño.
    /// </summary>
    public LicitacionesDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable(
            "LICITACIONES_DESIGN_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Defina LICITACIONES_DESIGN_CONNECTION_STRING para usar las herramientas de EF Core.");
        }

        var options = new DbContextOptionsBuilder<LicitacionesDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new LicitacionesDbContext(options);
    }
}
