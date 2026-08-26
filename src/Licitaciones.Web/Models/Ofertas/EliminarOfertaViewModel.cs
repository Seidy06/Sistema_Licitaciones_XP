namespace Licitaciones.Web.Models.Ofertas;

public sealed class EliminarOfertaViewModel
{
    public Guid Id { get; init; }
    public Guid LicitacionId { get; init; }
    public string ProveedorNombre { get; init; } = string.Empty;
    public decimal Monto { get; init; }
    public string Moneda { get; init; } = string.Empty;
    public DateTimeOffset FechaRegistro { get; init; }
}
