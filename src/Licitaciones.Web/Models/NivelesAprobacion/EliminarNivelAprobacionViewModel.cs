namespace Licitaciones.Web.Models.NivelesAprobacion;

public sealed record EliminarNivelAprobacionViewModel(
    int Id,
    string Nombre,
    decimal MontoMinimo,
    decimal? MontoMaximo);
