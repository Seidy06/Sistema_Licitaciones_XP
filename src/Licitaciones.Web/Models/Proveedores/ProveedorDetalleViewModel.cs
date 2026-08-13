namespace Licitaciones.Web.Models.Proveedores;

public sealed record ProveedorDetalleViewModel(
    Guid Id,
    string Nombre,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
