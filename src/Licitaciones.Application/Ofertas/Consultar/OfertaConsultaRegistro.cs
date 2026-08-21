namespace Licitaciones.Application.Ofertas.Consultar;

public sealed record OfertaConsultaRegistro(
    Guid Id,
    Guid LicitacionId,
    string ProveedorNombre,
    decimal Monto,
    DateTimeOffset FechaRegistro);

