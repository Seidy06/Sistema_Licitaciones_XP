namespace Licitaciones.Application.Ofertas.Consultar;

public sealed record OfertaConsultaDto(
    Guid Id,
    string ProveedorNombre,
    decimal Monto,
    string Moneda,
    DateTimeOffset FechaRegistro,
    bool EsMejorOferta);

