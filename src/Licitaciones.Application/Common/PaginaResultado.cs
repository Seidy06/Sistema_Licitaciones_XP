namespace Licitaciones.Application.Common;

/// <summary>
/// Resultado genérico paginado para consultas con soporte de paginación.
/// </summary>
/// <typeparam name="TipoElemento">Tipo de los elementos en la página.</typeparam>
public sealed record PaginaResultado<T>(
    IReadOnlyList<T> Items,
    int Total,
    int Pagina,
    int TamanoPagina);
