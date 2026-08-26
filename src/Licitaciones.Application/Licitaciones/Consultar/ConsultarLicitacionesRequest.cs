using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Licitaciones.Consultar;

/// <summary>
/// Parámetros de consulta para filtrar y paginar licitaciones.
/// </summary>
public sealed record ConsultarLicitacionesRequest(
    EstadoLicitacion? EstadoFiltro = null,
    string? Codigo = null,
    DateTimeOffset? FechaDesde = null,
    DateTimeOffset? FechaHasta = null,
    string OrdenarPor = "fechaCierre",
    bool Descendente = false,
    int Pagina = 1,
    int TamanoPagina = 20);
