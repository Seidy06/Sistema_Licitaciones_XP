using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Web.Models.NivelesAprobacion;

public sealed class EditarNivelAprobacionViewModel : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del nivel de aprobación es obligatorio.")]
    [StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [Range(0, double.MaxValue,
        ErrorMessage = "El monto mínimo no puede ser negativo.")]
    public decimal MontoMinimo { get; set; }

    public decimal? MontoMaximo { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (MontoMaximo.HasValue && MontoMaximo.Value <= MontoMinimo)
        {
            yield return new ValidationResult(
                "El monto máximo debe ser mayor que el monto mínimo.",
                new[] { nameof(MontoMaximo) });
        }
    }
}
