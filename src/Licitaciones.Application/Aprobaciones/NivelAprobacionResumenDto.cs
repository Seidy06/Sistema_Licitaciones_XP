namespace Licitaciones.Application.Aprobaciones;

public sealed record NivelAprobacionResumenDto(
    int Id,
    string Nombre,
    decimal MontoMinimo,
    decimal? MontoMaximo,
    bool Activo);
