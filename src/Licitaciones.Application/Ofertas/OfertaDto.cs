using Licitaciones.Domain.Ofertas;

namespace Licitaciones.Application.Ofertas;

/// <summary>
/// DTO que representa los datos de una oferta para transferencia entre capas.
/// </summary>
public sealed record OfertaDto(
    Guid Id,
    Guid LicitacionId,
    Guid ProveedorId,
    decimal Monto,
    DateTimeOffset FechaRegistro)
{
    public static OfertaDto FromEntity(Oferta oferta) => new(
        oferta.Id,
        oferta.LicitacionId,
        oferta.ProveedorId,
        oferta.Monto,
        oferta.FechaRegistro);
}
