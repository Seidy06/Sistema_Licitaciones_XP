using Licitaciones.Domain.Common;

namespace Licitaciones.Domain.Aprobaciones;

/// <summary>
/// Entidad que define un nivel de aprobación según un rango de montos.
/// </summary>
public sealed class NivelAprobacion : IAuditableEntity
{
    private NivelAprobacion()
    {
    }

    /// <summary>
    /// Identificador del nivel de aprobación.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Nombre descriptivo del nivel de aprobación.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;

    /// <summary>
    /// Monto mínimo del rango para este nivel.
    /// </summary>
    public decimal MontoMinimo { get; private set; }

    /// <summary>
    /// Monto máximo del rango para este nivel, o null si no tiene límite superior.
    /// </summary>
    public decimal? MontoMaximo { get; private set; }

    /// <summary>
    /// Indica si el nivel está activo y disponible para uso.
    /// </summary>
    public bool Activo { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; private set; }

    /// <inheritdoc />
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Crea un nuevo nivel de aprobación con el rango de montos especificado.
    /// </summary>
    /// <param name="nombre">Nombre descriptivo del nivel.</param>
    /// <param name="montoMinimo">Monto mínimo del rango.</param>
    /// <param name="montoMaximo">Monto máximo del rango, o null si no aplica.</param>
    /// <returns>Nueva instancia de <see cref="NivelAprobacion"/>.</returns>
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

    /// <summary>
    /// Desactiva el nivel de aprobación.
    /// </summary>
    public void Desactivar()
    {
        Activo = false;
    }

    /// <summary>
    /// Actualiza el nombre y el rango de montos del nivel de aprobación.
    /// </summary>
    /// <param name="nombre">Nuevo nombre descriptivo.</param>
    /// <param name="montoMinimo">Nuevo monto mínimo.</param>
    /// <param name="montoMaximo">Nuevo monto máximo, o null si no aplica.</param>
    public void Actualizar(
        string nombre,
        decimal montoMinimo,
        decimal? montoMaximo)
    {
        Validar(nombre, montoMinimo, montoMaximo);
        Nombre = nombre.Trim();
        MontoMinimo = montoMinimo;
        MontoMaximo = montoMaximo;
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
