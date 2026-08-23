using Licitaciones.Application.Ofertas.Consultar;
using Licitaciones.Application.Ofertas.Crear;
using Licitaciones.Application.Common;
using Licitaciones.Domain.Common;
using Licitaciones.Web.Models.Ofertas;

using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

public sealed class OfertasController : Controller
{
    private readonly ConsultarOfertaService _consultarService;
    private readonly CrearOfertaService _crearService;

    public OfertasController(
        ConsultarOfertaService consultarService,
        CrearOfertaService crearService)
    {
        _consultarService = consultarService;
        _crearService = crearService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        Guid licitacionId,
        string? proveedor = null,
        int pagina = 1,
        int tamanoPagina = 20,
        string ordenarPor = "monto",
        bool descendente = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var resultado = await _consultarService.ListarAsync(
                new ConsultarOfertasRequest(
                    licitacionId,
                    "CRC",
                    proveedor,
                    ordenarPor,
                    descendente,
                    pagina,
                    tamanoPagina),
                cancellationToken);

            var model = new PaginaResultado<OfertaItemViewModel>(
                resultado.Items
                    .Select(oferta => new OfertaItemViewModel(
                        oferta.Id,
                        oferta.ProveedorNombre,
                        oferta.Monto,
                        oferta.FechaRegistro))
                    .ToArray(),
                resultado.Total,
                resultado.Pagina,
                resultado.TamanoPagina);

            ViewData["LicitacionId"] = licitacionId == Guid.Empty ? null : licitacionId;
            ViewData["Proveedor"] = proveedor;
            ViewData["OrdenarPor"] = ordenarPor;
            ViewData["Descendente"] = descendente;
            return View(model);
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(new PaginaResultado<OfertaItemViewModel>(
                Array.Empty<OfertaItemViewModel>(), 0, pagina, tamanoPagina));
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
}
