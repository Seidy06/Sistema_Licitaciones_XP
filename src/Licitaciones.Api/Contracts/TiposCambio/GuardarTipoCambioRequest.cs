using System.ComponentModel.DataAnnotations;

namespace Licitaciones.Api.Contracts.TiposCambio;

/// <summary>
/// Contrato HTTP para crear o actualizar un tipo de cambio.
/// </summary>
public sealed record GuardarTipoCambioRequest(
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "El valor del tipo de cambio debe ser mayor a cero.")]
    decimal Valor,
    DateOnly Fecha);
