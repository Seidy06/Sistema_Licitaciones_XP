using Licitaciones.Domain.Proveedores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Licitaciones.Infrastructure.Persistence.Configurations;

public sealed class ProveedorConfiguration
    : IEntityTypeConfiguration<Proveedor>
{
    internal const string IndiceUnicoNombreNormalizado =
        "UX_Proveedores_NombreNormalizado";

    public void Configure(EntityTypeBuilder<Proveedor> builder)
    {
        builder.ToTable("Proveedores");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Nombre)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.NombreNormalizado)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(x => x.UpdatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.HasIndex(x => x.NombreNormalizado)
            .IsUnique()
            .HasDatabaseName(IndiceUnicoNombreNormalizado);

        builder.Property(x => x.Version)
            .IsRowVersion();
    }
}
