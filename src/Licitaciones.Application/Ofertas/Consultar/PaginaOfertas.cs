namespace Licitaciones.Application.Ofertas.Consultar;

public sealed record PaginaOfertas(
    IReadOnlyList<OfertaConsultaDto> Items,
    int Total,
    int Pagina,
    int TamanoPagina);
