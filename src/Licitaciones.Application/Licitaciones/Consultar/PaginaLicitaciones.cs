namespace Licitaciones.Application.Licitaciones.Consultar;

public sealed record PaginaLicitaciones(
    IReadOnlyList<LicitacionConsultaDto> Items,
    int Total,
    int Pagina,
    int TamanoPagina);
