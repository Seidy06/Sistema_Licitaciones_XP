using Licitaciones.Domain.Proveedores;

namespace Licitaciones.Application.Proveedores.Eliminar;

public interface IProveedorBajaRepository
{
    Task<Proveedor?> ObtenerActivoParaDarDeBajaAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task ActualizarBajaAsync(
        Proveedor proveedor,
        CancellationToken cancellationToken = default);
}
