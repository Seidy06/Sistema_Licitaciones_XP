using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;
using Licitaciones.Domain.Proveedores;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Licitaciones.Infrastructure.Persistence.Configurations;

public sealed class OfertaConfiguration : IEntityTypeConfiguration<Oferta>
{
    public const string IndiceUnicoLicitacionProveedor =
        "IX_Ofertas_LicitacionId_ProveedorId";

    public void Configure(EntityTypeBuilder<Oferta> builder)
    {
        builder.ToTable("Ofertas", table =>
            table.HasCheckConstraint("CK_Ofertas_Monto_Positivo", "\"Monto\" > 0"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Monto).HasColumnType("numeric(18,2)");
        builder.Property(x => x.FechaRegistro).HasColumnType("timestamp with time zone");
        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.Version).IsRowVersion();
        builder
            .HasIndex(x => new { x.LicitacionId, x.ProveedorId })
            .IsUnique()
            .HasDatabaseName(IndiceUnicoLicitacionProveedor);
        builder.HasOne<Licitacion>().WithMany().HasForeignKey(x => x.LicitacionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Proveedor>().WithMany().HasForeignKey(x => x.ProveedorId).OnDelete(DeleteBehavior.Restrict);
    }
}
