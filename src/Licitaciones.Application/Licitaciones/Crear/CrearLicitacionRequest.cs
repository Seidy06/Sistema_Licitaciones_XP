namespace Licitaciones.Application.Licitaciones.Crear;

/// <summary>
/// Datos requeridos para crear una nueva licitación.
/// </summary>
public sealed record CrearLicitacionRequest(
    string Codigo,
    string Titulo,
    decimal Presupuesto,
    DateTimeOffset FechaCierre);
