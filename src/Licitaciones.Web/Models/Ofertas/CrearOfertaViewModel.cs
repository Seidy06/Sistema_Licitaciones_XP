using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Web.Models.Ofertas;

public sealed class CrearOfertaViewModel
{
    [Required(ErrorMessage = "Seleccione una licitación.")]
    public Guid? LicitacionId { get; set; }

    [Required(ErrorMessage = "Seleccione un proveedor.")]
    public Guid? ProveedorId { get; set; }

    [Range(0.01, double.MaxValue,
        ErrorMessage = "El monto de la oferta debe ser mayor que cero.")]
    public decimal Monto { get; set; }
}
