namespace Licitaciones.Application.TiposCambio;

public sealed record TipoCambioDto(
    int Id,
    string MonedaOrigen,
    string MonedaDestino,
    decimal Valor,
    DateOnly Fecha,
    bool Activo);
