using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Api.Contracts.Proveedores;

/// <summary>
/// Contrato HTTP para crear un nuevo proveedor.
/// </summary>
public sealed record CrearProveedorRequest
{
    /// <summary>Nombre del proveedor. Obligatorio, máximo 200 caracteres.</summary>
    [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
    [StringLength(200, ErrorMessage = "El nombre del proveedor no puede exceder 200 caracteres.")]
    public string Nombre { get; init; } = string.Empty;
}
