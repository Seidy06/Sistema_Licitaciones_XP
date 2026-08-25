using Licitaciones.Domain.Common;

namespace Licitaciones.Domain.Ofertas;

/// <summary>
/// Entidad que representa una oferta presentada por un proveedor para una licitación.
/// </summary>
public sealed class Oferta : IAuditableEntity
{
    private Oferta()
    {
    }

    /// <summary>
    /// Identificador único de la oferta.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Identificador de la licitación a la que pertenece la oferta.
    /// </summary>
    public Guid LicitacionId { get; private set; }

    /// <summary>
    /// Identificador del proveedor que presenta la oferta.
    /// </summary>
    public Guid ProveedorId { get; private set; }

    /// <summary>
    /// Monto monetario ofrecido por el proveedor.
    /// </summary>
    public decimal Monto { get; private set; }

    /// <summary>
    /// Fecha y hora en que se registró la oferta.
    /// </summary>
    public DateTimeOffset FechaRegistro { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Fecha de eliminación lógica, o null si la oferta está activa.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; private set; }

    /// <summary>
    /// Número de versión para control de concurrencia optimista.
    /// </summary>
    public uint Version { get; private set; }

    /// <summary>
    /// Indica si la oferta ha sido eliminada lógicamente.
    /// </summary>
    public bool EstaEliminado => DeletedAt.HasValue;

    /// <summary>
    /// Crea una nueva oferta para una licitación.
    /// </summary>
    /// <param name="licitacionId">Identificador de la licitación.</param>
    /// <param name="proveedorId">Identificador del proveedor oferente.</param>
    /// <param name="monto">Monto ofrecido (debe ser mayor que cero).</param>
    /// <param name="clock">Reloj del sistema para registrar la fecha.</param>
    /// <returns>Nueva instancia de <see cref="Oferta"/>.</returns>
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

    /// <summary>
    /// Actualiza el monto de la oferta. No permite editar ofertas eliminadas.
    /// </summary>
    /// <param name="monto">Nuevo monto (debe ser mayor que cero).</param>
    /// <param name="clock">Reloj del sistema.</param>
    public void Editar(decimal monto, IClock clock)
    {
        if (EstaEliminado)
        {
            throw new DomainException("No se puede editar una oferta eliminada.");
        }

        if (monto <= 0)
        {
            throw new DomainException("El monto de la oferta debe ser mayor que cero.");
        }

        Monto = monto;
    }

    /// <summary>
    /// Elimina lógicamente la oferta.
    /// </summary>
    /// <param name="fecha">Fecha y hora de la eliminación.</param>
    public void Eliminar(DateTimeOffset fecha)
    {
        if (EstaEliminado)
        {
            throw new DomainException("La oferta ya fue eliminada.");
        }

        DeletedAt = fecha;
    }
}
