namespace Licitaciones.Domain.Licitaciones;

public sealed class LicitacionTransicion
{
    private LicitacionTransicion()
    {
    }

    public Guid Id { get; private set; }
    public Guid LicitacionId { get; private set; }
    public EstadoLicitacion EstadoAnterior { get; private set; }
    public EstadoLicitacion EstadoNuevo { get; private set; }
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
