using Licitaciones.Domain.Common;

namespace Licitaciones.Infrastructure.Time;

/// <summary>
/// Implementación del reloj del sistema que retorna la hora UTC actual.
/// </summary>
public sealed class SystemClock : IClock
{
    /// <summary>
    /// Obtiene la fecha y hora actual en UTC.
    /// </summary>
    public DateTimeOffset UtcNow() => DateTimeOffset.UtcNow;
}
