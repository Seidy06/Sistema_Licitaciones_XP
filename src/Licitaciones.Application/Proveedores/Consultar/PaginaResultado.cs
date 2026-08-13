namespace Licitaciones.Application.Proveedores.Consultar;

public sealed record PaginaResultado<T>(
    IReadOnlyList<T> Items,
    int Total,
    int Pagina,
    int TamanoPagina);
