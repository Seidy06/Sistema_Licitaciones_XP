using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Api.Contracts.Licitaciones;

/// <summary>
/// Contrato HTTP para cambiar el estado de una licitación via PATCH.
/// </summary>
public sealed record CambiarEstadoRequest(
    [Required(ErrorMessage = "El estado es obligatorio.")]
    string Estado);
