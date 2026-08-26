namespace Licitaciones.Application.Licitaciones.Consultar;

/// <summary>
/// Resultado paginado de la consulta de licitaciones.
/// </summary>
public sealed record PaginaLicitaciones(
    IReadOnlyList<LicitacionConsultaDto> Items,
    int Total,
    int Pagina,
    int TamanoPagina);
