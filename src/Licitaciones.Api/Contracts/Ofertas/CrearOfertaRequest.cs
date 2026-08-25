using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Api.Contracts.Ofertas;

/// <summary>
/// Contrato HTTP para crear una nueva oferta asociada a una licitación.
/// </summary>
public sealed class CrearOfertaRequest
{
    /// <summary>Identificador de la licitación.</summary>
    [Required]
    public Guid LicitacionId { get; init; }

    /// <summary>Identificador del proveedor.</summary>
    [Required]
    public Guid ProveedorId { get; init; }

    /// <summary>Monto ofertado. Debe ser mayor a cero.</summary>
    [Range(0.01, double.MaxValue)]
    public decimal Monto { get; init; }
}
