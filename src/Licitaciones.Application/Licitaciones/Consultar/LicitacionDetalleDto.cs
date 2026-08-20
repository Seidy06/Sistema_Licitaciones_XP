namespace Licitaciones.Application.Licitaciones.Consultar;

public sealed record LicitacionDetalleDto(
    Guid Id,
    string Codigo,
    string Titulo,
    decimal Presupuesto,
    DateTimeOffset FechaCierre,
    LicitacionMejorOfertaDto? MejorOferta,
    string? MensajeMejorOferta,
    LicitacionNivelAprobacionDto? NivelAprobacion);

public sealed record LicitacionMejorOfertaDto(
    Guid Id,
    decimal Monto,
    decimal AhorroPorcentaje,
    string Clasificacion);
