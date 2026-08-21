using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Api.Contracts.Aprobaciones;

public sealed class GuardarNivelAprobacionRequest
{
    [Required]
    [StringLength(100)]
    public string Nombre { get; init; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal MontoMinimo { get; init; }

    public decimal? MontoMaximo { get; init; }
}
