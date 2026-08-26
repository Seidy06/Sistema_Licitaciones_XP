using Licitaciones.Domain.Licitaciones;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Licitaciones.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework para las transiciones de estado de licitaciones.
/// </summary>
public sealed class LicitacionTransicionConfiguration
    : IEntityTypeConfiguration<LicitacionTransicion>
{
    public void Configure(EntityTypeBuilder<LicitacionTransicion> builder)
    {
        builder.ToTable("licitacion_transiciones");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(x => x.LicitacionId).HasColumnName("licitacion_id");
        builder.Property(x => x.EstadoAnterior).HasColumnName("estado_anterior");
        builder.Property(x => x.EstadoNuevo).HasColumnName("estado_nuevo");
        builder.Property(x => x.Fecha)
            .HasColumnName("fecha")
            .HasColumnType("timestamp with time zone");

        builder.HasOne<Licitacion>()
            .WithMany(x => x.Transiciones)
            .HasForeignKey(x => x.LicitacionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
