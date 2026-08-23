namespace Licitaciones.Application.Common;

public sealed record PaginaResultado<T>(
    IReadOnlyList<T> Items,
    int Total,
    int Pagina,
    int TamanoPagina);
