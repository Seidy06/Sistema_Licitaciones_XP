using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Web.Models.Ofertas;

public sealed class CrearOfertaViewModel
{
    [Required(ErrorMessage = "La licitación es obligatoria.")]
    public Guid? LicitacionId { get; set; }

    [Required(ErrorMessage = "El proveedor es obligatorio.")]
    public Guid? ProveedorId { get; set; }

    [Range(0.01, double.MaxValue,
        ErrorMessage = "El monto de la oferta debe ser mayor que cero.")]
    public decimal Monto { get; set; }
}
