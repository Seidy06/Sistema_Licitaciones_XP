using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Web.Models.TiposCambio;

public sealed class CrearTipoCambioViewModel
{
    [Range(0.01, double.MaxValue,
        ErrorMessage = "El valor del tipo de cambio debe ser mayor que cero.")]
    public decimal Valor { get; set; }

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    [DataType(DataType.Date)]
    public DateTime? Fecha { get; set; }
}
