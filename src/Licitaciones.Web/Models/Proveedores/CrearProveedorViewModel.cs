using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Web.Models.Proveedores;

public sealed class CrearProveedorViewModel
{
    [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
    [StringLength(200, ErrorMessage = "El nombre del proveedor no puede exceder 200 caracteres.")]
    [RegularExpression(
        @"^[\p{L}\p{N} .,\(\)]+$",
        ErrorMessage = "El nombre del proveedor contiene caracteres no permitidos.")]
    public string Nombre { get; set; } = string.Empty;
}
