using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Api.Contracts.Proveedores;

public sealed record CrearProveedorRequest
{
    [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
    [StringLength(200, ErrorMessage = "El nombre del proveedor no puede exceder 200 caracteres.")]
    [RegularExpression(
        @"^[\p{L}\p{N} .,\(\)]+$",
        ErrorMessage = "El nombre del proveedor contiene caracteres no permitidos.")]
    public string Nombre { get; init; } = string.Empty;
}
