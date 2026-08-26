using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;

namespace Licitaciones.Application.Ofertas.Eliminar;

/// <summary>
/// Puerto de salida para las operaciones de eliminación de ofertas.
/// </summary>
public interface IEliminarOfertaRepository
{
    /// <summary>
    /// Obtiene una oferta por su identificador.
    /// </summary>
    Task<Oferta?> ObtenerPorIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene la licitación asociada a una oferta.
    /// </summary>
    Task<Licitacion?> ObtenerLicitacionPorIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persiste todos los cambios pendientes en el repositorio.
    /// </summary>
    Task GuardarCambiosAsync(CancellationToken cancellationToken = default);
}
