namespace Licitaciones.Application.Aprobaciones;

/// <summary>
/// DTO con el resumen de un nivel de aprobación para consulta.
/// </summary>
public sealed record NivelAprobacionResumenDto(
    int Id,
    string Nombre,
    decimal MontoMinimo,
    decimal? MontoMaximo,
    bool Activo);
