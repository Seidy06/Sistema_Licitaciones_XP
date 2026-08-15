namespace Licitaciones.Application.Proveedores.Consultar;

public sealed record ProveedorHistoricoDto(
    Guid Id,
    string Nombre,
    string NombreNormalizado,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset DeletedAt,
    uint Version);
