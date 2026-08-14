using Licitaciones.Domain.Proveedores;

namespace Licitaciones.Application.Proveedores.Editar;

public sealed class EditarProveedorService
{
    private readonly IProveedorRepository _repository;

    public EditarProveedorService(IProveedorRepository repository)
    {
        _repository = repository;
    }

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
