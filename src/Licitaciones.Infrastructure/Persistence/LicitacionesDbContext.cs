using Licitaciones.Domain.Aprobaciones;
using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;
using Licitaciones.Domain.Proveedores;
using Licitaciones.Domain.TiposCambio;
using Licitaciones.Infrastructure.Time;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Licitaciones.Infrastructure.Persistence;

public sealed class LicitacionesDbContext : DbContext
{
    private readonly IClock _clock;

    public LicitacionesDbContext(
        DbContextOptions<LicitacionesDbContext> options,
        IClock? clock = null)
        : base(options)
    {
        _clock = clock ?? new SystemClock();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w =>
            w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<Licitacion> Licitaciones => Set<Licitacion>();
    public DbSet<LicitacionTransicion> LicitacionTransiciones => Set<LicitacionTransicion>();
    public DbSet<Oferta> Ofertas => Set<Oferta>();
    public DbSet<NivelAprobacion> NivelesAprobacion => Set<NivelAprobacion>();
    public DbSet<TipoCambio> TiposCambio => Set<TipoCambio>();
    public DbSet<EstadoLicitacionCatalogo> EstadosLicitacion => Set<EstadoLicitacionCatalogo>();

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        AplicarMarcasDeTiempo();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        AplicarMarcasDeTiempo();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LicitacionesDbContext).Assembly);
    }

    private void AplicarMarcasDeTiempo()
    {
        var instante = _clock.UtcNow();
        var ahora = new DateTimeOffset(
            instante.Ticks - (instante.Ticks % 10),
            instante.Offset);

        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property(nameof(IAuditableEntity.CreatedAt)).CurrentValue = ahora;
                entry.Property(nameof(IAuditableEntity.UpdatedAt)).CurrentValue = ahora;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Property(nameof(IAuditableEntity.CreatedAt)).IsModified = false;
                entry.Property(nameof(IAuditableEntity.UpdatedAt)).CurrentValue = ahora;
            }
        }
    }
}
