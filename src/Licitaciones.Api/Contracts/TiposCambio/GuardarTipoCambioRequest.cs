namespace Licitaciones.Api.Contracts.TiposCambio;

/// <summary>
/// Contrato HTTP para crear o actualizar un tipo de cambio.
/// </summary>
public sealed record GuardarTipoCambioRequest(decimal Valor, DateOnly Fecha);
