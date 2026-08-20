using Licitaciones.Domain.Ofertas;

namespace Licitaciones.Application.Ofertas;

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
