namespace Licitaciones.Domain.Common;

/// <summary>
/// Abstracción del reloj del sistema para permitir pruebas determinísticas.
/// </summary>
public interface IClock
{
    /// <summary>
    /// Obtiene la fecha y hora actual en UTC.
    /// </summary>
    DateTimeOffset UtcNow();
}
