using Licitaciones.Domain.TiposCambio;

namespace Licitaciones.Application.TiposCambio;

public interface ITipoCambioRepository
{
    Task<TipoCambio?> ObtenerActivoAsync(
        CancellationToken cancellationToken = default);

    Task ReemplazarActivoAsync(
        TipoCambio tipoCambio,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TipoCambio>> ListarTodosAsync(
        CancellationToken cancellationToken = default);
}
