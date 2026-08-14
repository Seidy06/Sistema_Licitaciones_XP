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

    Task<Proveedor?> ObtenerParaEditarAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    Task<bool> ExisteNombreNormalizadoAsync(
        string nombreNormalizado,
        Guid excluirProveedorId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    Task ActualizarAsync(
        Proveedor proveedor,
        uint versionEsperada,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();
}
