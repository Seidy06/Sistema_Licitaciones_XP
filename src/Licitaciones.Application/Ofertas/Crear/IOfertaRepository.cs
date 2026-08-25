using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;
using Licitaciones.Domain.Proveedores;

namespace Licitaciones.Application.Ofertas.Crear;

/// <summary>
/// Puerto de salida para la persistencia de ofertas en el contexto de creación.
/// </summary>
public interface IOfertaRepository
{
    /// <summary>
    /// Obtiene una licitación por su identificador.
    /// </summary>
    Task<Licitacion?> ObtenerLicitacionPorIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un proveedor por su identificador.
    /// </summary>
    Task<Proveedor?> ObtenerProveedorPorIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica si ya existe una oferta del proveedor para la licitación indicada.
    /// </summary>
    Task<bool> ExisteOfertaAsync(
        Guid licitacionId, Guid proveedorId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega una nueva oferta al repositorio.
    /// </summary>
    Task AgregarAsync(
        Oferta oferta, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persiste todos los cambios pendientes en el repositorio.
    /// </summary>
    Task GuardarCambiosAsync(
        CancellationToken cancellationToken = default);
}
