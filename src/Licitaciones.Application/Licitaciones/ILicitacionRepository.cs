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

    Task<Licitacion?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<decimal?> ObtenerMontoMinimoOfertaAsync(
        Guid licitacionId,
        CancellationToken cancellationToken = default);

    Task GuardarCambiosAsync(
        CancellationToken cancellationToken = default);
}
