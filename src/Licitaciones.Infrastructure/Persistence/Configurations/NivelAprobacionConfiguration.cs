using Licitaciones.Domain.Aprobaciones;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Licitaciones.Infrastructure.Persistence.Configurations;

public sealed class NivelAprobacionConfiguration : IEntityTypeConfiguration<NivelAprobacion>
{
    private static readonly DateTimeOffset SeedDate = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public void Configure(EntityTypeBuilder<NivelAprobacion> builder)
    {
        builder.ToTable("NivelesAprobacion", table =>
        {
            table.HasCheckConstraint("CK_NivelesAprobacion_Minimo", "\"MontoMinimo\" >= 0");
            table.HasCheckConstraint(
                "CK_NivelesAprobacion_Rango",
                "\"MontoMaximo\" IS NULL OR \"MontoMaximo\" > \"MontoMinimo\"");
        });
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("nextval('\"NivelesAprobacion_Id_seq\"'::regclass)");
        builder.Property(x => x.Nombre).IsRequired().HasMaxLength(100);
        builder.Property(x => x.MontoMinimo).HasColumnType("numeric(18,2)");
        builder.Property(x => x.MontoMaximo).HasColumnType("numeric(18,2)");
        builder.Property(x => x.Activo).HasDefaultValue(true);
        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone");

        builder.HasData(
            new { Id = 1, Nombre = "Encargado de area", MontoMinimo = 0.01m, MontoMaximo = (decimal?)999_999.99m, Activo = true, CreatedAt = SeedDate, UpdatedAt = SeedDate },
            new { Id = 2, Nombre = "Gerencia", MontoMinimo = 1_000_000m, MontoMaximo = (decimal?)9_999_999.99m, Activo = true, CreatedAt = SeedDate, UpdatedAt = SeedDate },
            new { Id = 3, Nombre = "Junta Directiva", MontoMinimo = 10_000_000m, MontoMaximo = (decimal?)null, Activo = true, CreatedAt = SeedDate, UpdatedAt = SeedDate });
    }
}
