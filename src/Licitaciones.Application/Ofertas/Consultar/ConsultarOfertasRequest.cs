namespace Licitaciones.Application.Ofertas.Consultar;

public sealed record ConsultarOfertasRequest(
    Guid LicitacionId,
    string Moneda = "CRC",
    string? Proveedor = null,
    string OrdenarPor = "monto",
    bool Descendente = false,
    int Pagina = 1,
    int TamanoPagina = 20);
