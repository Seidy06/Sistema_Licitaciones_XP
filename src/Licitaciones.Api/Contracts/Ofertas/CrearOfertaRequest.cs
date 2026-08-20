using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Api.Contracts.Ofertas;

public sealed class CrearOfertaRequest
{
    [Required]
    public Guid LicitacionId { get; init; }

    [Required]
    public Guid ProveedorId { get; init; }

    [Range(0.01, double.MaxValue)]
    public decimal Monto { get; init; }
}
