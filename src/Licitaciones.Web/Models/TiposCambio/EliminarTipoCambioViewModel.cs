namespace Licitaciones.Web.Models.TiposCambio;

public sealed class EliminarTipoCambioViewModel
{
    public int Id { get; init; }
    public string MonedaOrigen { get; init; } = string.Empty;
    public string MonedaDestino { get; init; } = string.Empty;
    public decimal Valor { get; init; }
    public DateOnly Fecha { get; init; }
    public bool Activo { get; init; }
}
