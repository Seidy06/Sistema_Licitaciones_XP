namespace Licitaciones.Application.Ofertas;

public sealed record OfertaDto(
    Guid Id,
    Guid LicitacionId,
    Guid ProveedorId,
    decimal Monto,
    DateTimeOffset FechaRegistro);
