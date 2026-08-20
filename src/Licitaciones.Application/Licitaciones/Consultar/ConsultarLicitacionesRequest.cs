using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Licitaciones.Consultar;

public sealed record ConsultarLicitacionesRequest(
    EstadoLicitacion? EstadoFiltro = null);
