namespace Licitaciones.Application.Ofertas.Consultar;

/// <summary>
/// Registro proyectado de una oferta para resultados de consulta.
/// </summary>
public sealed record OfertaConsultaRegistro(
    Guid Id,
    Guid LicitacionId,
    string ProveedorNombre,
    decimal Monto,
    DateTimeOffset FechaRegistro);

