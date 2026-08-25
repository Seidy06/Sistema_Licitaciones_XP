using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Api.Contracts.Aprobaciones;

/// <summary>
/// Contrato HTTP para crear o actualizar un nivel de aprobación.
/// </summary>
public sealed class GuardarNivelAprobacionRequest
{
    /// <summary>Nombre del nivel de aprobación. Obligatorio, máximo 100 caracteres.</summary>
    [Required]
    [StringLength(100)]
    public string Nombre { get; init; } = string.Empty;

    /// <summary>Monto mínimo del rango. Debe ser mayor o igual a cero.</summary>
    [Range(0, double.MaxValue)]
    public decimal MontoMinimo { get; init; }

    /// <summary>Monto máximo del rango. Null indica sin límite superior.</summary>
    public decimal? MontoMaximo { get; init; }
}
