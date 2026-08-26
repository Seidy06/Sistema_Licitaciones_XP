using Licitaciones.Domain.Common;
using Licitaciones.Domain.Ofertas;

namespace Licitaciones.Application.Licitaciones.Consultar;

/// <summary>
/// Servicio para consultar licitaciones paginadas y obtener detalles con mejor oferta.
/// </summary>
public sealed class ConsultarLicitacionService
{
    private readonly ILicitacionConsultaRepository _repository;

    public ConsultarLicitacionService(ILicitacionConsultaRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Lista licitaciones paginadas según los filtros de búsqueda.
    /// </summary>
    /// <param name="consulta">Parámetros de filtrado y paginación.</param>
    /// <param name="clock">Reloj para determinar el estado efectivo.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Página de resultados con las licitaciones encontradas.</returns>
    public async Task<PaginaLicitaciones> ListarAsync(
        ConsultarLicitacionesRequest consulta,
        IClock clock,
        CancellationToken cancellationToken = default)
    {
        ValidarConsulta(consulta);
        var licitaciones = await _repository.ListarAsync(consulta, cancellationToken);

        var items = licitaciones
            .Skip((consulta.Pagina - 1) * consulta.TamanoPagina)
            .Take(consulta.TamanoPagina)
            .Select(l => new LicitacionConsultaDto(
                l.Id,
                l.Codigo,
                l.Titulo,
                l.Presupuesto,
                l.FechaCierre,
                l.EstadoEfectivo(clock)))
            .ToArray();

        return new PaginaLicitaciones(
            items, licitaciones.Count, consulta.Pagina, consulta.TamanoPagina);
    }

    /// <summary>
    /// Obtiene el detalle de una licitación con su mejor oferta y nivel de aprobación.
    /// </summary>
    /// <param name="id">Identificador de la licitación.</param>
    /// <param name="clock">Reloj para determinar el estado efectivo.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Detalle de la licitación o null si no existe.</returns>
    public async Task<LicitacionDetalleDto?> ObtenerDetalleAsync(
        Guid id,
        IClock clock,
        CancellationToken cancellationToken = default)
    {
        var licitacion = await _repository.ObtenerPorIdAsync(id, cancellationToken);

        if (licitacion is null)
        {
            return null;
        }

        var ofertas = await _repository.ObtenerOfertasAsync(
            licitacion.Id, cancellationToken);
        var resultado = CalculadoraMejorOferta.Calcular(
            licitacion.Presupuesto,
            ofertas);

        LicitacionMejorOfertaDto? mejorOferta = resultado is not null
            ? new LicitacionMejorOfertaDto(
                resultado.Id,
                resultado.Monto,
                resultado.AhorroPorcentaje,
                resultado.Clasificacion)
            : null;

        LicitacionNivelAprobacionDto? nivelAprobacion = resultado is not null
            ? await _repository.ObtenerNivelAprobacionAsync(
                resultado.Monto, cancellationToken)
            : null;

        return new LicitacionDetalleDto(
            licitacion.Id,
            licitacion.Codigo,
            licitacion.Titulo,
            licitacion.Presupuesto,
            licitacion.FechaCierre,
            mejorOferta,
            resultado is null ? "Sin ofertas válidas" : null,
            nivelAprobacion);
    }

    private static void ValidarConsulta(ConsultarLicitacionesRequest consulta)
    {
        if (consulta.Pagina <= 0 || consulta.TamanoPagina is <= 0 or > 100)
        {
            throw new DomainException("La paginaciÃ³n solicitada no es vÃ¡lida.");
        }

        if (consulta.FechaDesde > consulta.FechaHasta)
        {
            throw new DomainException("El rango de fechas no es vÃ¡lido.");
        }

        if (consulta.OrdenarPor.ToLowerInvariant() is not ("fechacierre" or "codigo" or "presupuesto"))
        {
            throw new DomainException("El campo de ordenamiento no es vÃ¡lido.");
        }
    }
}
