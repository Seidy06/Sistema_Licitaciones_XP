namespace Licitaciones.Application.TiposCambio;

/// <summary>
/// DTO que representa los datos de un tipo de cambio para transferencia entre capas.
/// </summary>
public sealed record TipoCambioDto(
    int Id,
    string MonedaOrigen,
    string MonedaDestino,
    decimal Valor,
    DateOnly Fecha,
    bool Activo);
