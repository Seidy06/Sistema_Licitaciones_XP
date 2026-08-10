namespace Licitaciones.Application.Proveedores;

public sealed record ProveedorDto(
    Guid Id,
    string Nombre,
    string NombreNormalizado,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    uint Version);
