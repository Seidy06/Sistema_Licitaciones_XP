namespace Licitaciones.Web.Models.Ofertas;

public sealed record OfertaItemViewModel(
    Guid Id,
    string ProveedorNombre,
    decimal Monto,
    string Moneda,
    bool EsMejorOferta,
    DateTimeOffset FechaRegistro);
