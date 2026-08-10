using Licitaciones.Domain.Proveedores;

namespace Licitaciones.Application.Proveedores;

public interface IProveedorRepository
{
    Task<bool> ExisteNombreNormalizadoAsync(
        string nombreNormalizado,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        Proveedor proveedor,
        CancellationToken cancellationToken = default);
}
