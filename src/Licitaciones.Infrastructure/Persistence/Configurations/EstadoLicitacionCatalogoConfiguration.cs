using Licitaciones.Domain.Licitaciones;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Licitaciones.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para el catálogo de estados de licitación.
/// </summary>
public sealed class EstadoLicitacionCatalogoConfiguration
    : IEntityTypeConfiguration<EstadoLicitacionCatalogo>
{
    public void Configure(EntityTypeBuilder<EstadoLicitacionCatalogo> builder)
    {
        builder.ToTable("EstadosLicitacion");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Nombre).IsRequired().HasMaxLength(50);
        builder.HasIndex(x => x.Nombre).IsUnique();

        builder.HasData(
            new { Id = EstadoLicitacion.Borrador, Nombre = "Borrador" },
            new { Id = EstadoLicitacion.Publicada, Nombre = "Publicada" },
            new { Id = EstadoLicitacion.Cerrada, Nombre = "Cerrada" },
            new { Id = EstadoLicitacion.Adjudicada, Nombre = "Adjudicada" },
            new { Id = EstadoLicitacion.Cancelada, Nombre = "Cancelada" });
    }
}
