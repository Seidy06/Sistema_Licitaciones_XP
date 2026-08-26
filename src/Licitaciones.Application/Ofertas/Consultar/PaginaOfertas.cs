namespace Licitaciones.Application.Ofertas.Consultar;

/// <summary>
/// Resultado paginado de la consulta de ofertas.
/// </summary>
public sealed record PaginaOfertas(
    IReadOnlyList<OfertaConsultaDto> Items,
    int Total,
    int Pagina,
    int TamanoPagina);
