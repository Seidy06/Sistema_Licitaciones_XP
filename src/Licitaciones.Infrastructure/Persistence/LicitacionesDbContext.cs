using Licitaciones.Domain.Proveedores;
using Microsoft.EntityFrameworkCore;

namespace Licitaciones.Infrastructure.Persistence;

public sealed class LicitacionesDbContext : DbContext
{
    public LicitacionesDbContext(DbContextOptions<LicitacionesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Proveedor> Proveedores => Set<Proveedor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LicitacionesDbContext).Assembly);
    }
}
