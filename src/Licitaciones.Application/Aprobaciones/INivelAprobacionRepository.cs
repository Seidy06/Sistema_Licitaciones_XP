using Licitaciones.Domain.Aprobaciones;

namespace Licitaciones.Application.Aprobaciones;

/// <summary>
/// Puerto de salida para la persistencia de niveles de aprobación.
/// </summary>
public interface INivelAprobacionRepository
{
    /// <summary>
    /// Verifica si existe un rango de montos traslapado con un nivel activo.
    /// </summary>
    Task<bool> ExisteTraslapeActivoAsync(
        decimal montoMinimo,
        decimal? montoMaximo,
        int? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un nuevo nivel de aprobación.
    /// </summary>
    Task AgregarAsync(
        NivelAprobacion nivel,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista todos los niveles de aprobación activos.
    /// </summary>
    Task<IReadOnlyList<NivelAprobacion>> ListarActivosAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un nivel de aprobación por su identificador.
    /// </summary>
    Task<NivelAprobacion?> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Persiste todos los cambios pendientes en el repositorio.
    /// </summary>
    Task GuardarCambiosAsync(
        CancellationToken cancellationToken = default);
}
