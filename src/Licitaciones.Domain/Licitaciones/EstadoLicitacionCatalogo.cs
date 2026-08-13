namespace Licitaciones.Domain.Licitaciones;

public sealed class EstadoLicitacionCatalogo
{
    private EstadoLicitacionCatalogo()
    {
    }

    public EstadoLicitacion Id { get; private set; }

    public string Nombre { get; private set; } = string.Empty;
}
