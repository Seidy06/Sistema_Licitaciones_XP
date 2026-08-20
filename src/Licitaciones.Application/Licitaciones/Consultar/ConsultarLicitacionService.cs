using Licitaciones.Domain.Common;
using Licitaciones.Domain.Ofertas;

namespace Licitaciones.Application.Licitaciones.Consultar;

public sealed class ConsultarLicitacionService
{
    private readonly ILicitacionConsultaRepository _repository;
    private readonly CalculadoraMejorOferta _calculadora = new();

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

        var ofertas = await _repository.ObtenerOfertasAsync(
            licitacion.Id, cancellationToken);
        var resultado = _calculadora.Calcular(licitacion.Presupuesto, ofertas);

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
}
