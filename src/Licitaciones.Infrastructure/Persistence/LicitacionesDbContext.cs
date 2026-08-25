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

/// <summary>
/// Contexto de Entity Framework para el dominio de licitaciones.
/// </summary>
public sealed class LicitacionesDbContext : DbContext
{
    private readonly IClock _clock;

    /// <summary>
    /// Inicializa una nueva instancia del contexto de licitaciones.
    /// </summary>
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

    /// <summary>Conjunto de entidades de proveedores.</summary>
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    /// <summary>Conjunto de entidades de licitaciones.</summary>
    public DbSet<Licitacion> Licitaciones => Set<Licitacion>();
    /// <summary>Conjunto de transiciones de estado de licitaciones.</summary>
    public DbSet<LicitacionTransicion> LicitacionTransiciones => Set<LicitacionTransicion>();
    /// <summary>Conjunto de entidades de ofertas.</summary>
    public DbSet<Oferta> Ofertas => Set<Oferta>();
    /// <summary>Conjunto de niveles de aprobación.</summary>
    public DbSet<NivelAprobacion> NivelesAprobacion => Set<NivelAprobacion>();
    /// <summary>Conjunto de tipos de cambio.</summary>
    public DbSet<TipoCambio> TiposCambio => Set<TipoCambio>();
    /// <summary>Conjunto del catálogo de estados de licitación.</summary>
    public DbSet<EstadoLicitacionCatalogo> EstadosLicitacion => Set<EstadoLicitacionCatalogo>();

    /// <inheritdoc />
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        AplicarMarcasDeTiempo();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    /// <inheritdoc />
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
