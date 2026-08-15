namespace Licitaciones.Web.Models.Proveedores;

public sealed record ProveedorHistoricoResumenViewModel(
    Guid Id,
    string Nombre,
    DateTimeOffset CreatedAt,
    DateTimeOffset DeletedAt);
