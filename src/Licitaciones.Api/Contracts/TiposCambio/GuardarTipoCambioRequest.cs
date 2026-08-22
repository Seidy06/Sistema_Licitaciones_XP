namespace Licitaciones.Api.Contracts.TiposCambio;

public sealed record GuardarTipoCambioRequest(decimal Valor, DateOnly Fecha);
