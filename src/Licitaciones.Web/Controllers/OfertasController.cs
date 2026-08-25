using Licitaciones.Application.Common;
using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Application.Ofertas.Consultar;
using Licitaciones.Application.Ofertas.Crear;
using Licitaciones.Application.Ofertas.Editar;
using Licitaciones.Application.Ofertas.Eliminar;
using Licitaciones.Domain.Common;
using Licitaciones.Web.Models.Ofertas;

using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

public sealed class OfertasController : Controller
{
    private const string MonedaPredeterminada = "CRC";

    private readonly ConsultarOfertaService _consultarService;
    private readonly CrearOfertaService _crearService;
    private readonly EditarOfertaService _editarService;
    private readonly EliminarOfertaService _eliminarService;
    private readonly ConsultarLicitacionService _consultarLicitacionService;
    private readonly IClock _clock;

    public OfertasController(
        ConsultarOfertaService consultarService,
        CrearOfertaService crearService,
        EditarOfertaService editarService,
        EliminarOfertaService eliminarService,
        ConsultarLicitacionService consultarLicitacionService,
        IClock clock)
    {
        _consultarService = consultarService;
        _crearService = crearService;
        _editarService = editarService;
        _eliminarService = eliminarService;
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
        if (!ModelState.IsValid) return View(model);

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

    [HttpGet]
    public async Task<IActionResult> Details(
        Guid id, string moneda = "CRC",
        CancellationToken cancellationToken = default)
    {
        var oferta = await _consultarService.ObtenerAsync(id, moneda, cancellationToken);
        if (oferta is null) return NotFound();

        var model = new DetalleOfertaViewModel
        {
            Id = oferta.Id,
            LicitacionId = oferta.LicitacionId,
            ProveedorNombre = oferta.ProveedorNombre,
            Monto = oferta.Monto,
            Moneda = oferta.Moneda,
            FechaRegistro = oferta.FechaRegistro,
            EsMejorOferta = oferta.EsMejorOferta,
            TipoCambioValor = oferta.TipoCambioValor,
            TipoCambioFecha = oferta.TipoCambioFecha
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        Guid id, CancellationToken cancellationToken = default)
    {
        var oferta = await _consultarService.ObtenerAsync(id, "CRC", cancellationToken);
        if (oferta is null) return NotFound();

        var model = new EditarOfertaViewModel
        {
            Id = oferta.Id,
            LicitacionId = oferta.LicitacionId,
            ProveedorNombre = oferta.ProveedorNombre,
            Monto = oferta.Monto,
            Moneda = oferta.Moneda
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id, EditarOfertaViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        try
        {
            await _editarService.EditarAsync(
                new EditarOfertaRequest(model.Id, model.Monto),
                cancellationToken);
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }

        TempData["MensajeExito"] = "La oferta se actualizó correctamente.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(
        Guid id, CancellationToken cancellationToken = default)
    {
        var oferta = await _consultarService.ObtenerAsync(id, "CRC", cancellationToken);
        if (oferta is null) return NotFound();

        var model = new EliminarOfertaViewModel
        {
            Id = oferta.Id,
            LicitacionId = oferta.LicitacionId,
            ProveedorNombre = oferta.ProveedorNombre,
            Monto = oferta.Monto,
            Moneda = oferta.Moneda,
            FechaRegistro = oferta.FechaRegistro
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var eliminada = await _eliminarService.EliminarAsync(id, cancellationToken);
            if (!eliminada) return NotFound();
        }
        catch (DomainException exception)
        {
            TempData["MensajeError"] = exception.Message;
            return RedirectToAction(nameof(Details), new { id });
        }

        TempData["MensajeExito"] = "La oferta fue eliminada.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<LicitacionMejorOfertaDto?> ObtenerMejorOfertaAsync(
        Guid licitacionId,
        CancellationToken cancellationToken)
    {
        if (licitacionId == Guid.Empty) return null;

        var detalle = await _consultarLicitacionService.ObtenerDetalleAsync(
            licitacionId, _clock, cancellationToken);
        return detalle?.MejorOferta;
    }
}
