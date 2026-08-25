using Licitaciones.Domain.Common;

namespace Licitaciones.Domain.Ofertas;

public sealed class Oferta : IAuditableEntity
{
    private Oferta()
    {
    }

    public Guid Id { get; private set; }
    public Guid LicitacionId { get; private set; }
    public Guid ProveedorId { get; private set; }
    public decimal Monto { get; private set; }
    public DateTimeOffset FechaRegistro { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public uint Version { get; private set; }

    public static Oferta Crear(
        Guid licitacionId,
        Guid proveedorId,
        decimal monto,
        IClock clock)
    {
        if (licitacionId == Guid.Empty || proveedorId == Guid.Empty)
        {
            throw new DomainException("La licitación y el proveedor son obligatorios.");
        }

        if (monto <= 0)
        {
            throw new DomainException("El monto de la oferta debe ser mayor que cero.");
        }

        return new Oferta
        {
            Id = Guid.NewGuid(),
            LicitacionId = licitacionId,
            ProveedorId = proveedorId,
            Monto = monto,
            FechaRegistro = clock.UtcNow()
        };
    }
}
