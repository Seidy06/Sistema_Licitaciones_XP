namespace Licitaciones.Application.Ofertas.Consultar;

/// <summary>
/// DTO con los datos de una oferta para consulta incluyendo conversión de moneda.
/// </summary>
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

