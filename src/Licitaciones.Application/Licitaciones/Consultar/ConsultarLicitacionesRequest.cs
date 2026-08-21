using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Licitaciones.Consultar;

public sealed record ConsultarLicitacionesRequest(
    EstadoLicitacion? EstadoFiltro = null,
    string? Codigo = null,
    DateTimeOffset? FechaDesde = null,
    DateTimeOffset? FechaHasta = null,
    string OrdenarPor = "fechaCierre",
    bool Descendente = false,
    int Pagina = 1,
    int TamanoPagina = 20);
