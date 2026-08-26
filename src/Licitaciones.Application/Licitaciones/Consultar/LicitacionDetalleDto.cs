namespace Licitaciones.Application.Licitaciones.Consultar;

/// <summary>
/// DTO con el detalle completo de una licitación incluyendo mejor oferta y nivel de aprobación.
/// </summary>
public sealed record LicitacionDetalleDto(
    Guid Id,
    string Codigo,
    string Titulo,
    decimal Presupuesto,
    DateTimeOffset FechaCierre,
    LicitacionMejorOfertaDto? MejorOferta,
    string? MensajeMejorOferta,
    LicitacionNivelAprobacionDto? NivelAprobacion);

/// <summary>
/// DTO con los datos de la mejor oferta asociada a una licitación.
/// </summary>
public sealed record LicitacionMejorOfertaDto(
    Guid Id,
    decimal Monto,
    decimal AhorroPorcentaje,
    string Clasificacion);
