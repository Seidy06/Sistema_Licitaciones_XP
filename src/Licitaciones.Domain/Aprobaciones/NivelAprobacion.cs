using Licitaciones.Domain.Common;

namespace Licitaciones.Domain.Aprobaciones;

public sealed class NivelAprobacion : IAuditableEntity
{
    private NivelAprobacion()
    {
    }

    public int Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public decimal MontoMinimo { get; private set; }
    public decimal? MontoMaximo { get; private set; }
    public bool Activo { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static NivelAprobacion Crear(
        string nombre,
        decimal montoMinimo,
        decimal? montoMaximo)
    {
        Validar(nombre, montoMinimo, montoMaximo);

        return new NivelAprobacion
        {
            Nombre = nombre.Trim(),
            MontoMinimo = montoMinimo,
            MontoMaximo = montoMaximo,
            Activo = true
        };
    }

    private static void Validar(
        string nombre,
        decimal montoMinimo,
        decimal? montoMaximo)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            throw new DomainException("El nombre del nivel de aprobación es obligatorio.");
        }

        if (montoMinimo < 0)
        {
            throw new DomainException("El monto mínimo no puede ser negativo.");
        }

        if (montoMaximo.HasValue && montoMaximo.Value <= montoMinimo)
        {
            throw new DomainException("El monto máximo debe ser mayor que el monto mínimo.");
        }
    }
}
