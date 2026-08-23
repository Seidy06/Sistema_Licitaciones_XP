namespace Licitaciones.Web.Models.Licitaciones;

public sealed record LicitacionItemViewModel(
    Guid Id,
    string Titulo,
    decimal Presupuesto,
    DateTimeOffset FechaCierre,
    string Estado);
