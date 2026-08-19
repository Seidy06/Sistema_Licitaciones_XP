using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Web.Models.Licitaciones;

public sealed class CrearLicitacionViewModel
{
    [Required]
    [StringLength(50)]
    public string Codigo { get; set; } = string.Empty;

    [Required]
    [StringLength(250)]
    public string Titulo { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Presupuesto { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime FechaCierre { get; set; }
}
