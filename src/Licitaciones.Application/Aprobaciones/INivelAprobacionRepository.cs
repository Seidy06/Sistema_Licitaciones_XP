using Licitaciones.Domain.Aprobaciones;

namespace Licitaciones.Application.Aprobaciones;

public interface INivelAprobacionRepository
{
    Task<bool> ExisteTraslapeActivoAsync(
        decimal montoMinimo,
        decimal? montoMaximo,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        NivelAprobacion nivel,
        CancellationToken cancellationToken = default);
}
