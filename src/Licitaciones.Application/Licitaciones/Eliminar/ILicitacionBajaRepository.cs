using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Licitaciones.Eliminar;

/// <summary>
/// Puerto de salida para las operaciones de baja de licitaciones.
/// </summary>
public interface ILicitacionBajaRepository
{
    /// <summary>
    /// Obtiene una licitación activa por su identificador para darla de baja.
    /// </summary>
    Task<Licitacion?> ObtenerActivaParaDarDeBajaAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza el estado de una licitación tras darla de baja.
    /// </summary>
    Task ActualizarBajaAsync(
        Licitacion licitacion,
        CancellationToken cancellationToken = default);
}
