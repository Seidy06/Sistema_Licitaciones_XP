using Licitaciones.Application.Common;
using Licitaciones.Application.Licitaciones.Consultar;

namespace Licitaciones.Web.Models.Ofertas;

public sealed record OfertasIndexViewModel(
    PaginaResultado<OfertaItemViewModel> Ofertas,
    LicitacionMejorOfertaDto? MejorOferta,
    string Moneda);
