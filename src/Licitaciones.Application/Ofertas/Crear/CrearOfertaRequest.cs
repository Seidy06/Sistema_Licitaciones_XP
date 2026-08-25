namespace Licitaciones.Application.Ofertas.Crear;

/// <summary>
/// Datos requeridos para crear una nueva oferta.
/// </summary>
public sealed record CrearOfertaRequest(
    Guid LicitacionId,
    Guid ProveedorId,
    decimal Monto);
