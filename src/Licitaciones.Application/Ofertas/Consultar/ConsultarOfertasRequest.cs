namespace Licitaciones.Application.Ofertas.Consultar;

/// <summary>
/// Parámetros de consulta para filtrar y paginar ofertas de una licitación.
/// </summary>
public sealed record ConsultarOfertasRequest(
    Guid? LicitacionId = null,
    string Moneda = "CRC",
    string? Proveedor = null,
    string OrdenarPor = "monto",
    bool Descendente = false,
    int Pagina = 1,
    int TamanoPagina = 20);
