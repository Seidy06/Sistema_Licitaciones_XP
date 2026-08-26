namespace Licitaciones.Domain.Licitaciones;

/// <summary>
/// Catálogo de valores del enumerado <see cref="EstadoLicitacion"/> con su nombre legible.
/// </summary>
public sealed class EstadoLicitacionCatalogo
{
    private EstadoLicitacionCatalogo()
    {
    }

    /// <summary>
    /// Identificador del estado.
    /// </summary>
    public EstadoLicitacion Id { get; private set; }

    /// <summary>
    /// Nombre legible del estado.
    /// </summary>
    public string Nombre { get; private set; } = string.Empty;
}
