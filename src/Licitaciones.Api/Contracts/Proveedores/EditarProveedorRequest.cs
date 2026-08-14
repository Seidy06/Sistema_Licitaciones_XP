using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Api.Contracts.Proveedores;

public sealed record EditarProveedorRequest
{
    [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
    [StringLength(200, ErrorMessage = "El nombre del proveedor no puede exceder 200 caracteres.")]
    public string Nombre { get; init; } = string.Empty;

    public uint Version { get; init; }
}
