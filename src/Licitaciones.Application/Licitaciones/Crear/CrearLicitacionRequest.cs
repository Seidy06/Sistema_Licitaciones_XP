namespace Licitaciones.Application.Licitaciones.Crear;

public sealed record CrearLicitacionRequest(
    string Codigo,
    string Titulo,
    decimal Presupuesto,
    DateTimeOffset FechaCierre);
