using Licitaciones.Domain.Common;

namespace Licitaciones.Application.Licitaciones.Consultar;

public sealed class ConsultarLicitacionService
{
    private readonly ILicitacionConsultaRepository _repository;

    public ConsultarLicitacionService(ILicitacionConsultaRepository repository)
    {
        _repository = repository;
    }

    public async Task<PaginaLicitaciones> ListarAsync(
        ConsultarLicitacionesRequest consulta,
        IClock clock,
        CancellationToken cancellationToken = default)
    {
        var licitaciones = await _repository.ListarAsync(consulta, cancellationToken);

        var items = licitaciones
            .Select(l => new LicitacionConsultaDto(
                l.Id,
                l.Titulo,
                l.Presupuesto,
                l.FechaCierre,
                l.EstadoEfectivo(clock)))
            .ToArray();

        return new PaginaLicitaciones(items);
    }

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

        var montoMinimo = await _repository.ObtenerMontoMinimoOfertaAsync(
            licitacion.Id, cancellationToken);

        LicitacionMejorOfertaDto? mejorOferta = montoMinimo.HasValue
            ? new LicitacionMejorOfertaDto(montoMinimo.Value)
            : null;

        LicitacionNivelAprobacionDto? nivelAprobacion = montoMinimo.HasValue
            ? await _repository.ObtenerNivelAprobacionAsync(
                montoMinimo.Value, cancellationToken)
            : null;

        return new LicitacionDetalleDto(
            licitacion.Id,
            licitacion.Codigo,
            licitacion.Titulo,
            licitacion.Presupuesto,
            licitacion.FechaCierre,
            mejorOferta,
            nivelAprobacion);
    }
}
