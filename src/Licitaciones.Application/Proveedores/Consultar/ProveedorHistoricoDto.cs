namespace Licitaciones.Application.Proveedores.Consultar;

/// <summary>
/// DTO con los datos históricos de un proveedor incluyendo fecha de baja.
/// </summary>
public sealed record ProveedorHistoricoDto(
    Guid Id,
    string Nombre,
    string NombreNormalizado,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset DeletedAt,
    uint Version);
