using Licitaciones.Application.Aprobaciones;
using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Domain.Common;
using Licitaciones.Web.Models.NivelesAprobacion;

using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

public sealed class NivelesAprobacionController : Controller
{
    private readonly AdministrarNivelesAprobacionService _service;

    public NivelesAprobacionController(AdministrarNivelesAprobacionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? nombre = null,
        int pagina = 1,
        int tamanoPagina = 20,
        string ordenarPor = "montoMinimo",
        bool descendente = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var resultado = await _service.ListarAsync(
                new NivelesAprobacionConsultaRequest(
                    nombre, ordenarPor, descendente, pagina, tamanoPagina),
                cancellationToken);

            var model = new PaginaResultado<NivelesAprobacionItemViewModel>(
                resultado.Items
                    .Select(nivel => new NivelesAprobacionItemViewModel(
                        nivel.Id,
                        nivel.Nombre,
                        nivel.MontoMinimo,
                        nivel.MontoMaximo,
                        nivel.Activo))
                    .ToArray(),
                resultado.Total,
                resultado.Pagina,
                resultado.TamanoPagina);

            ViewData["Nombre"] = nombre;
            ViewData["OrdenarPor"] = ordenarPor;
            ViewData["Descendente"] = descendente;
            return View(model);
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(new PaginaResultado<NivelesAprobacionItemViewModel>(
                Array.Empty<NivelesAprobacionItemViewModel>(), 0, pagina, tamanoPagina));
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CrearNivelAprobacionViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CrearNivelAprobacionViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _service.CrearAsync(
                model.Nombre,
                model.MontoMinimo,
                model.MontoMaximo,
                cancellationToken);
        }
        catch (NivelAprobacionConflictoException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Ya existe un nivel de aprobación activo cuyo rango se traslape con el indicado.");
            return View(model);
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }

        TempData["MensajeExito"] = "El nivel de aprobación se creó correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var nivel = await _service.ObtenerPorIdAsync(id, cancellationToken);
        if (nivel is null)
        {
            return NotFound();
        }

        return View(new EliminarNivelAprobacionViewModel(
            nivel.Id, nivel.Nombre, nivel.MontoMinimo, nivel.MontoMaximo));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        int id,
        CancellationToken cancellationToken)
    {
        var desactivado = await _service.DesactivarAsync(id, cancellationToken);
        if (!desactivado)
        {
            return NotFound();
        }

        TempData["MensajeExito"] = "El nivel de aprobación fue desactivado.";
        return RedirectToAction(nameof(Index));
    }
}
