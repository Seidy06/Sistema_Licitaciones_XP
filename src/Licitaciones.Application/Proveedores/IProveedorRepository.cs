using Licitaciones.Domain.Proveedores;

namespace Licitaciones.Application.Proveedores;

/// <summary>
/// Puerto de salida para la persistencia de proveedores en el contexto de escritura.
/// </summary>
public interface IProveedorRepository
{
    /// <summary>
    /// Verifica si ya existe un proveedor con el nombre normalizado indicado.
    /// </summary>
    Task<bool> ExisteNombreNormalizadoAsync(
        string nombreNormalizado,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Agrega un nuevo proveedor al repositorio.
    /// </summary>
    Task AgregarAsync(
        Proveedor proveedor,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un proveedor por su identificador para edición.
    /// </summary>
    Task<Proveedor?> ObtenerParaEditarAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    /// <summary>
    /// Verifica si ya existe un proveedor con el nombre normalizado, excluyendo uno específico.
    /// </summary>
    Task<bool> ExisteNombreNormalizadoAsync(
        string nombreNormalizado,
        Guid excluirProveedorId,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    /// <summary>
    /// Actualiza un proveedor con control de concurrencia optimista.
    /// </summary>
    Task ActualizarAsync(
        Proveedor proveedor,
        uint versionEsperada,
        CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();
}
