namespace Licitaciones.Web.Models.Ofertas;

public sealed class DetalleOfertaViewModel
{
    public Guid Id { get; init; }
    public Guid LicitacionId { get; init; }
    public string ProveedorNombre { get; init; } = string.Empty;
    public decimal Monto { get; init; }
    public string Moneda { get; init; } = string.Empty;
    public DateTimeOffset FechaRegistro { get; init; }
    public bool EsMejorOferta { get; init; }
    public decimal? TipoCambioValor { get; init; }
    public DateOnly? TipoCambioFecha { get; init; }
}
