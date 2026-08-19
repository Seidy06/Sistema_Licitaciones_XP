using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Licitaciones;

public sealed record LicitacionDto(
    Guid Id,
    string Codigo,
    string CodigoNormalizado,
    string Titulo,
    decimal Presupuesto,
    DateTimeOffset FechaCierre,
    EstadoLicitacion Estado,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
