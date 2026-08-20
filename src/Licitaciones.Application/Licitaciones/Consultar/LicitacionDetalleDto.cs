namespace Licitaciones.Application.Licitaciones.Consultar;

public sealed record LicitacionDetalleDto(
    Guid Id,
    string Codigo,
    string Titulo,
    decimal Presupuesto,
    DateTimeOffset FechaCierre,
    LicitacionMejorOfertaDto? MejorOferta,
    LicitacionNivelAprobacionDto? NivelAprobacion);

public sealed record LicitacionMejorOfertaDto(decimal Monto);
