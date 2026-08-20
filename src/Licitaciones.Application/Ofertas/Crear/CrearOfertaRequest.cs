namespace Licitaciones.Application.Ofertas.Crear;

public sealed record CrearOfertaRequest(
    Guid LicitacionId,
    Guid ProveedorId,
    decimal Monto);
