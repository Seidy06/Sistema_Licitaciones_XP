using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Licitaciones;

/// <summary>
/// Puerto de salida para la persistencia de licitaciones en el contexto de escritura.
/// </summary>
public interface ILicitacionRepository
{
    /// <summary>
    /// Verifica si ya existe una licitación con el código normalizado indicado.
    /// </summary>
    Task<bool> ExisteCodigoNormalizadoAsync(
        string codigoNormalizado,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una nueva licitación al repositorio.
    /// </summary>
    Task AgregarAsync(
        Licitacion licitacion,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene una licitación por su identificador.
    /// </summary>
    Task<Licitacion?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene el monto mínimo de las ofertas registradas para una licitación.
    /// </summary>
    Task<decimal?> ObtenerMontoMinimoOfertaAsync(
        Guid licitacionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Persiste todos los cambios pendientes en el repositorio.
    /// </summary>
    Task GuardarCambiosAsync(
        CancellationToken cancellationToken = default);
}
