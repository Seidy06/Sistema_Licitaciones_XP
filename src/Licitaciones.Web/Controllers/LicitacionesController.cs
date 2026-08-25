using Licitaciones.Application.Common;
using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Application.Licitaciones.Crear;
using Licitaciones.Application.Licitaciones.Editar;
using Licitaciones.Application.Licitaciones.Publicar;
using Licitaciones.Application.Ofertas.Consultar;
using Licitaciones.Domain.Common;
using Licitaciones.Web.Models.Licitaciones;

using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

public sealed class LicitacionesController : Controller
{
    private readonly CrearLicitacionService _crearService;
    private readonly ConsultarLicitacionService _consultarService;
    private readonly PublicarLicitacionService _publicarService;
    private readonly EditarLicitacionService _editarService;
    private readonly ConsultarOfertaService _consultarOfertaService;
    private readonly IClock _clock;

    public LicitacionesController(
        CrearLicitacionService crearService,
        ConsultarLicitacionService consultarService,
        PublicarLicitacionService publicarService,
        EditarLicitacionService editarService,
        ConsultarOfertaService consultarOfertaService,
        IClock clock)
    {
        _crearService = crearService;
        _consultarService = consultarService;
        _publicarService = publicarService;
        _editarService = editarService;
        _consultarOfertaService = consultarOfertaService;
        _clock = clock;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? codigo = null,
        int pagina = 1,
        int tamanoPagina = 20,
        string ordenarPor = "fechaCierre",
        bool descendente = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var resultado = await _consultarService.ListarAsync(
                new ConsultarLicitacionesRequest(
                    null, codigo, null, null,
                    ordenarPor, descendente, pagina, tamanoPagina),
                _clock, cancellationToken);

            var model = new PaginaResultado<LicitacionItemViewModel>(
                resultado.Items
                    .Select(licitacion => new LicitacionItemViewModel(
                        licitacion.Id, licitacion.Titulo, licitacion.Presupuesto,
                        licitacion.FechaCierre, licitacion.EstadoEfectivo.ToString()))
                    .ToArray(),
                resultado.Total, resultado.Pagina, resultado.TamanoPagina);

            ViewData["Codigo"] = codigo;
            ViewData["OrdenarPor"] = ordenarPor;
            ViewData["Descendente"] = descendente;
            return View(model);
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(new PaginaResultado<LicitacionItemViewModel>(
                Array.Empty<LicitacionItemViewModel>(), 0, pagina, tamanoPagina));
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        Guid id, CancellationToken cancellationToken = default)
    {
        var detalle = await _consultarService.ObtenerDetalleAsync(id, _clock, cancellationToken);
        if (detalle is null) return NotFound();

        var model = new DetalleLicitacionViewModel
        {
            Id = detalle.Id,
            Codigo = detalle.Codigo,
            Titulo = detalle.Titulo,
            Presupuesto = detalle.Presupuesto,
            FechaCierre = detalle.FechaCierre,
            MejorOferta = detalle.MejorOferta is not null
                ? new LicitacionMejorOfertaViewModel
                {
                    Id = detalle.MejorOferta.Id,
                    Monto = detalle.MejorOferta.Monto,
                    AhorroPorcentaje = detalle.MejorOferta.AhorroPorcentaje,
                    Clasificacion = detalle.MejorOferta.Clasificacion
                }
                : null,
            MensajeMejorOferta = detalle.MensajeMejorOferta,
            NivelAprobacion = detalle.NivelAprobacion is not null
                ? new LicitacionNivelAprobacionViewModel
                {
                    Id = detalle.NivelAprobacion.Id,
                    Nombre = detalle.NivelAprobacion.Nombre
                }
                : null
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Create() => View(new CrearLicitacionViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CrearLicitacionViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            await _crearService.CrearAsync(
                new CrearLicitacionRequest(
                    model.Codigo, model.Titulo, model.Presupuesto,
                    new DateTimeOffset(model.FechaCierre).ToUniversalTime()),
                cancellationToken);
        }
        catch (LicitacionDuplicadoException exception)
        {
            ModelState.AddModelError(nameof(model.Codigo), exception.Message);
            return View(model);
        }

        TempData["MensajeExito"] = "La licitación se creó correctamente.";
        return RedirectToAction(nameof(Create));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(
        Guid id, CancellationToken cancellationToken = default)
    {
        var detalle = await _consultarService.ObtenerDetalleAsync(id, _clock, cancellationToken);
        if (detalle is null) return NotFound();

        var model = new EditarLicitacionViewModel
        {
            Id = detalle.Id,
            Codigo = detalle.Codigo,
            Titulo = detalle.Titulo,
            Presupuesto = detalle.Presupuesto,
            FechaCierre = detalle.FechaCierre.UtcDateTime
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id, EditarLicitacionViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        try
        {
            await _editarService.EditarAsync(
                new EditarLicitacionRequest(
                    model.Id, model.Codigo, model.Titulo,
                    model.Presupuesto,
                    new DateTimeOffset(model.FechaCierre).ToUniversalTime()),
                cancellationToken);
        }
        catch (LicitacionNoEncontradaException)
        {
            return NotFound();
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }

        TempData["MensajeExito"] = "La licitación se actualizó correctamente.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publicar(
        Guid id, string? codigo = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _publicarService.PublicarAsync(id, cancellationToken);
        }
        catch (LicitacionNoEncontradaException)
        {
            return NotFound();
        }
        catch (DomainException exception)
        {
            TempData["MensajeError"] = exception.Message;
        }

        return RedirectToAction(nameof(Index), new { codigo });
    }

    [HttpGet]
    public async Task<IActionResult> Ofertas(
        Guid id, CancellationToken cancellationToken = default)
    {
        var detalle = await _consultarService.ObtenerDetalleAsync(id, _clock, cancellationToken);
        if (detalle is null) return NotFound();

        try
        {
            var resultado = await _consultarOfertaService.ListarAsync(
                new ConsultarOfertasRequest(id),
                cancellationToken);

            ViewData["LicitacionCodigo"] = detalle.Codigo;
            ViewData["LicitacionId"] = id;
            return View(resultado);
        }
        catch (DomainException exception)
        {
            TempData["MensajeError"] = exception.Message;
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
