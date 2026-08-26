using Licitaciones.Domain.Proveedores;

namespace Licitaciones.Application.Proveedores.Editar;

/// <summary>
/// Servicio para editar proveedores existentes con validación de nombre duplicado.
/// </summary>
public sealed class EditarProveedorService
{
    private readonly IProveedorRepository _repository;

    public EditarProveedorService(IProveedorRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Actualiza el nombre de un proveedor con control de concurrencia.
    /// </summary>
    /// <param name="id">Identificador del proveedor a editar.</param>
    /// <param name="request">Datos a actualizar del proveedor.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO con los datos actualizados del proveedor.</returns>
    public async Task<ProveedorDto> EditarAsync(
        Guid id,
        EditarProveedorRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var proveedor = await _repository.ObtenerParaEditarAsync(id, cancellationToken)
            ?? throw new ProveedorNoEncontradoException(id);
        var nombreNormalizado = ProveedorNombreNormalizer.Normalizar(request.Nombre);

        if (await _repository.ExisteNombreNormalizadoAsync(
                nombreNormalizado, id, cancellationToken))
        {
            throw new ProveedorDuplicadoException(request.Nombre);
        }

        if (proveedor.NombreNormalizado != nombreNormalizado)
        {
            proveedor.Editar(request.Nombre);
        }

        await _repository.ActualizarAsync(proveedor, request.Version, cancellationToken);

        return new ProveedorDto(
            proveedor.Id,
            proveedor.Nombre,
            proveedor.NombreNormalizado,
            proveedor.CreatedAt,
            proveedor.UpdatedAt,
            proveedor.Version);
    }
}
