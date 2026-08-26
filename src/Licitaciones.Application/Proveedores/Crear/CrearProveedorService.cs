using Licitaciones.Domain.Proveedores;

namespace Licitaciones.Application.Proveedores.Crear;

/// <summary>
/// Servicio para crear nuevos proveedores con validación de nombre duplicado.
/// </summary>
public sealed class CrearProveedorService
{
    private readonly IProveedorRepository _repository;

    public CrearProveedorService(IProveedorRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Registra un proveedor nuevo validando que el nombre no esté duplicado.
    /// </summary>
    /// <param name="request">Datos del proveedor a crear.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO con los datos del proveedor creado.</returns>
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
