using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Licitaciones.Consultar;

/// <summary>
/// DTO con los datos resumidos de una licitación para resultados de consulta.
/// </summary>
public sealed record LicitacionConsultaDto(
    Guid Id,
    string Codigo,
    string Titulo,
    decimal Presupuesto,
    DateTimeOffset FechaCierre,
    EstadoLicitacion EstadoEfectivo);
