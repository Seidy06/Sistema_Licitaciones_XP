namespace Licitaciones.Web.Models.Proveedores;

public sealed record ProveedorResumenViewModel(
    Guid Id,
    string Nombre,
    DateTimeOffset CreatedAt);
