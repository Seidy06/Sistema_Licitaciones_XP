namespace Licitaciones.Application.Ofertas.Editar;

/// <summary>
/// Datos para actualizar el monto de una oferta existente.
/// </summary>
public sealed record EditarOfertaRequest(
    Guid Id,
    decimal Monto);
