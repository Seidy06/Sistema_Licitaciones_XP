namespace Licitaciones.Domain.Licitaciones;

/// <summary>
/// Registro inmutable de una transición de estado en el ciclo de vida de una licitación.
/// </summary>
public sealed class LicitacionTransicion
{
    private LicitacionTransicion()
    {
    }

    /// <summary>
    /// Identificador único de la transición.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Identificador de la licitación a la que pertenece esta transición.
    /// </summary>
    public Guid LicitacionId { get; private set; }

    /// <summary>
    /// Estado previo a la transición.
    /// </summary>
    public EstadoLicitacion EstadoAnterior { get; private set; }

    /// <summary>
    /// Estado resultante de la transición.
    /// </summary>
    public EstadoLicitacion EstadoNuevo { get; private set; }

    /// <summary>
    /// Fecha y hora en que ocurrió la transición.
    /// </summary>
    public DateTimeOffset Fecha { get; private set; }

    internal static LicitacionTransicion Crear(
        Guid licitacionId,
        EstadoLicitacion estadoAnterior,
        EstadoLicitacion estadoNuevo,
        DateTimeOffset fecha) =>
        new()
        {
            Id = Guid.NewGuid(),
            LicitacionId = licitacionId,
            EstadoAnterior = estadoAnterior,
            EstadoNuevo = estadoNuevo,
            Fecha = fecha.ToUniversalTime()
        };
}
