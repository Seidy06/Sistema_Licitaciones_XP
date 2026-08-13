using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Web.Models.Proveedores;

public sealed class CrearProveedorViewModel
{
    [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
    [StringLength(200, ErrorMessage = "El nombre del proveedor no puede exceder 200 caracteres.")]
    public string Nombre { get; set; } = string.Empty;
}
