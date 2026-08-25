namespace Licitaciones.Domain.Common;

/// <summary>
/// Interfaz para entidades que registran timestamps de auditoría.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Fecha y hora de creación del registro.
    /// </summary>
    DateTimeOffset CreatedAt { get; }

    /// <summary>
    /// Fecha y hora de la última actualización del registro.
    /// </summary>
    DateTimeOffset UpdatedAt { get; }
}
