namespace Licitaciones.Application.Proveedores;

/// <summary>
/// DTO que representa los datos de un proveedor para transferencia entre capas.
/// </summary>
public sealed record ProveedorDto(
    Guid Id,
    string Nombre,
    string NombreNormalizado,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    uint Version);
