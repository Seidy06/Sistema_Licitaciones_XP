namespace Licitaciones.Application.Licitaciones;

public sealed record EditarLicitacionRequest(
    Guid Id,
    string? Codigo,
    string? Titulo,
    decimal? Presupuesto,
    DateTimeOffset? FechaCierre);
