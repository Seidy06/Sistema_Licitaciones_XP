using Licitaciones.Application.Common;

namespace Licitaciones.Web.Models.Proveedores;

public sealed class ProveedorOfertasViewModel
{
    public ProveedorResumenViewModel Proveedor { get; init; } = null!;
    public PaginaResultado<ProveedorOfertaItemViewModel> Ofertas { get; init; } = null!;
    public string Moneda { get; init; } = "CRC";
}

public sealed record ProveedorOfertaItemViewModel(
    Guid Id,
    string LicitacionCodigo,
    decimal Monto,
    string Moneda,
    DateTimeOffset FechaRegistro);
