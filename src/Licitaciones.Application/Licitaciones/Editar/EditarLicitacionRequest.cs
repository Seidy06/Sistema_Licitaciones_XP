namespace Licitaciones.Application.Licitaciones.Editar;

public sealed record EditarLicitacionRequest(
    Guid Id,
    string? Codigo,
    string? Titulo,
    decimal? Presupuesto,
    DateTimeOffset? FechaCierre);
