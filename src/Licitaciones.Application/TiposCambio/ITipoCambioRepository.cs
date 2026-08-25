using Licitaciones.Domain.TiposCambio;

namespace Licitaciones.Application.TiposCambio;

/// <summary>
/// Puerto de salida para la persistencia de tipos de cambio.
/// </summary>
public interface ITipoCambioRepository
{
    /// <summary>
    /// Obtiene el tipo de cambio actualmente activo.
    /// </summary>
    Task<TipoCambio?> ObtenerActivoAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un tipo de cambio por su identificador.
    /// </summary>
    Task<TipoCambio?> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reemplaza el tipo de cambio activo con uno nuevo.
    /// </summary>
    Task ReemplazarActivoAsync(
        TipoCambio tipoCambio,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista todos los tipos de cambio registrados.
    /// </summary>
    Task<IReadOnlyList<TipoCambio>> ListarTodosAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Persiste todos los cambios pendientes en el repositorio.
    /// </summary>
    Task GuardarCambiosAsync(
        CancellationToken cancellationToken = default);
}
