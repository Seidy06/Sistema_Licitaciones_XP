using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Web.Models.Ofertas;

public sealed class EditarOfertaViewModel
{
    public Guid Id { get; set; }
    public Guid LicitacionId { get; set; }
    public string ProveedorNombre { get; set; } = string.Empty;
    public string Moneda { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue,
        ErrorMessage = "El monto de la oferta debe ser mayor que cero.")]
    public decimal Monto { get; set; }
}
