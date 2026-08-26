using Licitaciones.Application.Aprobaciones;
using Licitaciones.Application.Common;
using Licitaciones.Domain.Common;
using Licitaciones.Web.Models.NivelesAprobacion;

using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

/// <summary>
/// Controlador MVC para la administración de niveles de aprobación de licitaciones.
/// </summary>
public sealed class NivelesAprobacionController : Controller
{
    private readonly AdministrarNivelesAprobacionService _service;

    /// <summary>
    /// Inicializa una nueva instancia del controlador de niveles de aprobación.
    /// </summary>
    public NivelesAprobacionController(AdministrarNivelesAprobacionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Muestra el listado paginado de niveles de aprobación con filtros de búsqueda.
    /// </summary>
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

    /// <summary>
    /// Muestra el formulario para crear un nuevo nivel de aprobación.
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CrearNivelAprobacionViewModel());
    }

    /// <summary>
    /// Procesa la creación de un nuevo nivel de aprobación con los datos del formulario.
    /// </summary>
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

    /// <summary>
    /// Muestra la confirmación de desactivación de un nivel de aprobación.
    /// </summary>
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

    /// <summary>
    /// Confirma la desactivación de un nivel de aprobación y redirige al listado.
    /// </summary>
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

    /// <summary>
    /// Muestra el detalle completo de un nivel de aprobación por su identificador.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(
        int id, CancellationToken cancellationToken = default)
    {
        var nivel = await _service.ObtenerPorIdAsync(id, cancellationToken);
        if (nivel is null) return NotFound();

        var model = new DetalleNivelAprobacionViewModel
        {
            Id = nivel.Id,
            Nombre = nivel.Nombre,
            MontoMinimo = nivel.MontoMinimo,
            MontoMaximo = nivel.MontoMaximo,
            Activo = nivel.Activo
        };

        return View(model);
    }

    /// <summary>
    /// Muestra el formulario de edición con los datos actuales del nivel de aprobación.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(
        int id, CancellationToken cancellationToken = default)
    {
        var nivel = await _service.ObtenerPorIdAsync(id, cancellationToken);
        if (nivel is null) return NotFound();

        var model = new EditarNivelAprobacionViewModel
        {
            Id = nivel.Id,
            Nombre = nivel.Nombre,
            MontoMinimo = nivel.MontoMinimo,
            MontoMaximo = nivel.MontoMaximo
        };

        return View(model);
    }

    /// <summary>
    /// Procesa la actualización de un nivel de aprobación existente.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id, EditarNivelAprobacionViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        try
        {
            var resultado = await _service.ActualizarAsync(
                id, model.Nombre, model.MontoMinimo, model.MontoMaximo, cancellationToken);

            if (resultado is null) return NotFound();
        }
        catch (NivelAprobacionConflictoException)
        {
            ModelState.AddModelError(string.Empty,
                "Ya existe un nivel de aprobación activo cuyo rango se traslape con el indicado.");
            return View(model);
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }

        TempData["MensajeExito"] = "El nivel de aprobación se actualizó correctamente.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
