namespace Licitaciones.Web.Models.TiposCambio;

public sealed record TipoCambioItemViewModel(
    int Id,
    string MonedaOrigen,
    string MonedaDestino,
    decimal Valor,
    DateOnly Fecha,
    bool Activo);
