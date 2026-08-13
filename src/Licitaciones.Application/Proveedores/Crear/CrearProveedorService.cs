using Licitaciones.Domain.Proveedores;

namespace Licitaciones.Application.Proveedores.Crear;

public sealed class CrearProveedorService
{
    private readonly IProveedorRepository _repository;

    public CrearProveedorService(IProveedorRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProveedorDto> CrearAsync(
        CrearProveedorRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var proveedor = Proveedor.Crear(request.Nombre);

        if (await _repository.ExisteNombreNormalizadoAsync(
                proveedor.NombreNormalizado,
                cancellationToken))
        {
            throw new ProveedorDuplicadoException(request.Nombre);
        }

        await _repository.AgregarAsync(proveedor, cancellationToken);

        return new ProveedorDto(
            proveedor.Id,
            proveedor.Nombre,
            proveedor.NombreNormalizado,
            proveedor.CreatedAt,
            proveedor.UpdatedAt,
            proveedor.Version);
    }
}
