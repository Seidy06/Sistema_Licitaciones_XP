namespace Licitaciones.Web.Models.Proveedores;

public sealed record ProveedorHistoricoDetalleViewModel(
    Guid Id,
    string Nombre,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt,
    DateTimeOffset DeletedAt);
