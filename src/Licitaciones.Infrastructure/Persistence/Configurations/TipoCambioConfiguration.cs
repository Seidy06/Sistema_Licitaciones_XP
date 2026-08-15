using Licitaciones.Domain.TiposCambio;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Licitaciones.Infrastructure.Persistence.Configurations;

public sealed class TipoCambioConfiguration : IEntityTypeConfiguration<TipoCambio>
{
    private static readonly DateTimeOffset SeedDate = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    public void Configure(EntityTypeBuilder<TipoCambio> builder)
    {
        builder.ToTable("TiposCambio", table =>
            table.HasCheckConstraint("CK_TiposCambio_Valor_Positivo", "\"Valor\" > 0"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.MonedaOrigen).IsRequired().HasMaxLength(3);
        builder.Property(x => x.MonedaDestino).IsRequired().HasMaxLength(3);
        builder.Property(x => x.Valor).HasColumnType("numeric(18,6)");
        builder.Property(x => x.Fecha).HasColumnType("date");
        builder.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone");
        builder.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone");
        builder.HasIndex(x => x.Activo)
            .IsUnique()
            .HasFilter("\"Activo\" = TRUE")
            .HasDatabaseName("UX_TiposCambio_Activo");

        builder.HasData(new
        {
            Id = 1,
            MonedaOrigen = "USD",
            MonedaDestino = "CRC",
            Valor = 500m,
            Fecha = new DateOnly(2026, 1, 1),
            Activo = true,
            CreatedAt = SeedDate,
            UpdatedAt = SeedDate
        });
    }
}
