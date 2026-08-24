using Licitaciones.Application.Common;
using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Application.Ofertas.Consultar;
using Licitaciones.Application.Ofertas.Crear;
using Licitaciones.Domain.Common;
using Licitaciones.Web.Models.Ofertas;

using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

public sealed class OfertasController : Controller
{
    private const string MonedaPredeterminada = "CRC";

    private readonly ConsultarOfertaService _consultarService;
    private readonly CrearOfertaService _crearService;
    private readonly ConsultarLicitacionService _consultarLicitacionService;
    private readonly IClock _clock;

    public OfertasController(
        ConsultarOfertaService consultarService,
        CrearOfertaService crearService,
        ConsultarLicitacionService consultarLicitacionService,
        IClock clock)
    {
        _consultarService = consultarService;
        _crearService = crearService;
        _consultarLicitacionService = consultarLicitacionService;
        _clock = clock;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        Guid licitacionId,
        string? moneda = null,
        string? proveedor = null,
        int pagina = 1,
        int tamanoPagina = 20,
        string ordenarPor = "monto",
        bool descendente = false,
        CancellationToken cancellationToken = default)
    {
        var monedaSeleccionada = string.IsNullOrWhiteSpace(moneda)
            ? MonedaPredeterminada
            : moneda.Trim().ToUpperInvariant();

        try
        {
            var resultado = await _consultarService.ListarAsync(
                new ConsultarOfertasRequest(
                    licitacionId,
                    monedaSeleccionada,
                    proveedor,
                    ordenarPor,
                    descendente,
                    pagina,
                    tamanoPagina),
                cancellationToken);

            var model = new OfertasIndexViewModel(
                new PaginaResultado<OfertaItemViewModel>(
                    resultado.Items
                        .Select(oferta => new OfertaItemViewModel(
                            oferta.Id,
                            oferta.ProveedorNombre,
                            oferta.Monto,
                            oferta.Moneda,
                            oferta.EsMejorOferta,
                            oferta.FechaRegistro))
                        .ToArray(),
                    resultado.Total,
                    resultado.Pagina,
                    resultado.TamanoPagina),
                await ObtenerMejorOfertaAsync(licitacionId, cancellationToken),
                monedaSeleccionada);

            ViewData["LicitacionId"] = licitacionId == Guid.Empty ? null : licitacionId;
            ViewData["Proveedor"] = proveedor;
            ViewData["OrdenarPor"] = ordenarPor;
            ViewData["Descendente"] = descendente;
            return View(model);
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(new OfertasIndexViewModel(
                new PaginaResultado<OfertaItemViewModel>(
                    Array.Empty<OfertaItemViewModel>(), 0, pagina, tamanoPagina),
                null,
                monedaSeleccionada));
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CrearOfertaViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CrearOfertaViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _crearService.CrearAsync(
                new CrearOfertaRequest(
                    model.LicitacionId!.Value,
                    model.ProveedorId!.Value,
                    model.Monto),
                cancellationToken);
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }

        TempData["MensajeExito"] = "La oferta se registró correctamente.";
        return RedirectToAction(nameof(Index), new { licitacionId = model.LicitacionId });
    }

    private async Task<LicitacionMejorOfertaDto?> ObtenerMejorOfertaAsync(
        Guid licitacionId,
        CancellationToken cancellationToken)
    {
        if (licitacionId == Guid.Empty)
        {
            return null;
        }

        var detalle = await _consultarLicitacionService.ObtenerDetalleAsync(
            licitacionId,
            _clock,
            cancellationToken);
        return detalle?.MejorOferta;
    }
}
