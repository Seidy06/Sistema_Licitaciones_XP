namespace Licitaciones.Api.Contracts.Licitaciones;

/// <summary>
/// Contrato HTTP para editar una licitación existente. Todos los campos son opcionales.
/// </summary>
public sealed record EditarLicitacionRequest(
    string? Codigo,
    string? Titulo,
    decimal? Presupuesto,
    DateTimeOffset? FechaCierre);
