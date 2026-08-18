using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Api.Contracts.Licitaciones;

public sealed record CrearLicitacionRequest(
    [Required] string Codigo,
    [Required] string Titulo,
    [Range(0.01, double.MaxValue)]
    decimal Presupuesto,
    DateTimeOffset FechaCierre);
