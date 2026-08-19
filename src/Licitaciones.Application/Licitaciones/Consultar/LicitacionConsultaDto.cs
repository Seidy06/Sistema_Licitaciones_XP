using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Licitaciones.Consultar;

public sealed record LicitacionConsultaDto(
    Guid Id,
    string Titulo,
    decimal Presupuesto,
    DateTimeOffset FechaCierre,
    EstadoLicitacion EstadoEfectivo);
