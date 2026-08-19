using Licitaciones.Domain.Licitaciones;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Licitaciones.Infrastructure.Persistence.Configurations;

public sealed class LicitacionConfiguration : IEntityTypeConfiguration<Licitacion>
{
    internal const string IndiceUnicoCodigoNormalizado =
        "UX_Licitaciones_CodigoNormalizado";

    public void Configure(EntityTypeBuilder<Licitacion> builder)
    {
        builder.ToTable("Licitaciones", table =>
            table.HasCheckConstraint("CK_Licitaciones_Presupuesto_Positivo", "\"Presupuesto\" > 0"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Codigo).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CodigoNormalizado).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Titulo).IsRequired().HasMaxLength(250);
        builder.Property(x => x.Presupuesto).HasColumnType("numeric(18,2)");
        builder.Property(x => x.FechaCierre).HasColumnType("timestamp with time zone");
        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.DeletedAt).HasColumnType("timestamp with time zone");
        builder.HasIndex(x => x.CodigoNormalizado)
            .IsUnique()
            .HasFilter("\"DeletedAt\" IS NULL")
            .HasDatabaseName(IndiceUnicoCodigoNormalizado);
        builder.HasOne<EstadoLicitacionCatalogo>()
            .WithMany()
            .HasForeignKey(x => x.Estado)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
