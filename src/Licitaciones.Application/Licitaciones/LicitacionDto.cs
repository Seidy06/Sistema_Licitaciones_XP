using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Licitaciones;

/// <summary>
/// DTO que representa los datos de una licitación para transferencia entre capas.
/// </summary>
public sealed record LicitacionDto(
    Guid Id,
    string Codigo,
    string CodigoNormalizado,
    string Titulo,
    decimal Presupuesto,
    DateTimeOffset FechaCierre,
    EstadoLicitacion Estado,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    public static LicitacionDto FromEntity(Licitacion licitacion) => new(
        licitacion.Id,
        licitacion.Codigo,
        licitacion.CodigoNormalizado,
        licitacion.Titulo,
        licitacion.Presupuesto,
        licitacion.FechaCierre,
        licitacion.Estado,
        licitacion.CreatedAt,
        licitacion.UpdatedAt);
}
