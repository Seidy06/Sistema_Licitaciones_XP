namespace Licitaciones.Application.Proveedores.Consultar;

/// <summary>
/// Parámetros de consulta para filtrar y paginar proveedores.
/// </summary>
public sealed record ConsultarProveedoresRequest
{
    public ConsultarProveedoresRequest(
        int pagina = 1,
        int tamanoPagina = 20,
        string? nombre = null,
        ProveedorOrden ordenarPor = ProveedorOrden.Nombre,
        bool descendente = false)
    {
        if (pagina < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pagina));
        }

        if (tamanoPagina is < 1 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(tamanoPagina));
        }

        Pagina = pagina;
        TamanoPagina = tamanoPagina;
        Nombre = string.IsNullOrWhiteSpace(nombre) ? null : nombre.Trim();
        OrdenarPor = ordenarPor;
        Descendente = descendente;
    }

    public int Pagina { get; }
    public int TamanoPagina { get; }
    public string? Nombre { get; }
    public ProveedorOrden OrdenarPor { get; }
    public bool Descendente { get; }
}
