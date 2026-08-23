namespace Licitaciones.Web.Models.NivelesAprobacion;

public sealed record NivelesAprobacionItemViewModel(
    int Id,
    string Nombre,
    decimal MontoMinimo,
    decimal? MontoMaximo,
    bool Activo);
