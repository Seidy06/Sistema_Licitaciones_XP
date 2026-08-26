using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Api.Contracts.Proveedores;

/// <summary>
/// Contrato HTTP para editar un proveedor existente.
/// </summary>
public sealed record EditarProveedorRequest
{
    /// <summary>Nombre actualizado del proveedor. Obligatorio, máximo 200 caracteres.</summary>
    [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
    [StringLength(200, ErrorMessage = "El nombre del proveedor no puede exceder 200 caracteres.")]
    public string Nombre { get; init; } = string.Empty;

    /// <summary>Número de versión para control de concurrencia optimista.</summary>
    public uint Version { get; init; }
}
