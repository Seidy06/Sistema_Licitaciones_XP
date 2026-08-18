using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Licitaciones;

public interface ILicitacionRepository
{
    Task<bool> ExisteCodigoNormalizadoAsync(
        string codigoNormalizado,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        Licitacion licitacion,
        CancellationToken cancellationToken = default);
}
