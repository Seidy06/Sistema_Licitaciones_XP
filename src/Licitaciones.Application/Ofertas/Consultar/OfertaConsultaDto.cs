namespace Licitaciones.Application.Ofertas.Consultar;

public sealed record OfertaConsultaDto(
    Guid Id,
    Guid LicitacionId,
    string ProveedorNombre,
    decimal Monto,
    string Moneda,
    DateTimeOffset FechaRegistro,
    bool EsMejorOferta,
    decimal? TipoCambioValor = null,
    DateOnly? TipoCambioFecha = null);

