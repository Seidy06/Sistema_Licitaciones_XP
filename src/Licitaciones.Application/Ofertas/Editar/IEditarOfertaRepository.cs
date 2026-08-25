using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;

namespace Licitaciones.Application.Ofertas.Editar;

/// <summary>
/// Puerto de salida para las operaciones de edición de ofertas.
/// </summary>
public interface IEditarOfertaRepository
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
