namespace Licitaciones.Application.Licitaciones.Editar;

/// <summary>
/// Datos para actualizar una licitación existente (campos opcionales).
/// </summary>
public sealed record EditarLicitacionRequest(
    Guid Id,
    string? Codigo,
    string? Titulo,
    decimal? Presupuesto,
    DateTimeOffset? FechaCierre);
