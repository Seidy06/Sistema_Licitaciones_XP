namespace Licitaciones.Api.Contracts.Licitaciones;

public sealed record EditarLicitacionRequest(
    string? Codigo,
    string? Titulo,
    decimal? Presupuesto,
    DateTimeOffset? FechaCierre);
