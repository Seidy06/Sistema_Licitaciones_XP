using Licitaciones.Domain.Proveedores;

namespace Licitaciones.Application.Proveedores.Eliminar;

/// <summary>
/// Puerto de salida para las operaciones de baja de proveedores.
/// </summary>
public interface IProveedorBajaRepository
{
    /// <summary>
    /// Obtiene un proveedor activo por su identificador para darlo de baja.
    /// </summary>
    Task<Proveedor?> ObtenerActivoParaDarDeBajaAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Actualiza el estado de un proveedor tras darlo de baja.
    /// </summary>
    Task ActualizarBajaAsync(
        Proveedor proveedor,
        CancellationToken cancellationToken = default);
}
