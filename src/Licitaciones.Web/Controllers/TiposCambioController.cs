using Licitaciones.Application.Common;
using Licitaciones.Application.TiposCambio;
using Licitaciones.Domain.Common;
using Licitaciones.Web.Models.TiposCambio;

using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

/// <summary>
/// Controlador MVC para la administración de tipos de cambio de moneda.
/// </summary>
public sealed class TiposCambioController : Controller
{
    private readonly AdministrarTipoCambioService _service;

    /// <summary>
    /// Inicializa una nueva instancia del controlador de tipos de cambio.
    /// </summary>
    public TiposCambioController(AdministrarTipoCambioService service)
    {
        _service = service;
    }

    /// <summary>
    /// Muestra el listado paginado de tipos de cambio con ordenamiento.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Index(
        int pagina = 1,
        int tamanoPagina = 20,
        string ordenarPor = "fecha",
        bool descendente = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var resultado = await _service.ListarAsync(
                ordenarPor, descendente, pagina, tamanoPagina, cancellationToken);

            var model = new PaginaResultado<TipoCambioItemViewModel>(
                resultado.Items
                    .Select(tipo => new TipoCambioItemViewModel(
                        tipo.Id,
                        tipo.MonedaOrigen,
                        tipo.MonedaDestino,
                        tipo.Valor,
                        tipo.Fecha,
                        tipo.Activo))
                    .ToArray(),
                resultado.Total,
                resultado.Pagina,
                resultado.TamanoPagina);

            ViewData["OrdenarPor"] = ordenarPor;
            ViewData["Descendente"] = descendente;
            return View(model);
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(new PaginaResultado<TipoCambioItemViewModel>(
                Array.Empty<TipoCambioItemViewModel>(), 0, pagina, tamanoPagina));
        }
    }

    /// <summary>
    /// Muestra el formulario para registrar un nuevo tipo de cambio.
    /// </summary>
    [HttpGet]
    public IActionResult Create()
    {
        return View(new CrearTipoCambioViewModel());
    }

    /// <summary>
    /// Procesa el registro de un nuevo tipo de cambio con los datos del formulario.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CrearTipoCambioViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _service.GuardarAsync(
                model.Valor,
                DateOnly.FromDateTime(model.Fecha!.Value),
                cancellationToken);
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }

        TempData["MensajeExito"] = "El tipo de cambio se registró correctamente.";
        return RedirectToAction(nameof(Create));
    }

    /// <summary>
    /// Muestra el detalle completo de un tipo de cambio por su identificador.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Details(
        int id, CancellationToken cancellationToken = default)
    {
        var tipo = await _service.ObtenerPorIdAsync(id, cancellationToken);
        if (tipo is null) return NotFound();

        var model = new DetalleTipoCambioViewModel
        {
            Id = tipo.Id,
            MonedaOrigen = tipo.MonedaOrigen,
            MonedaDestino = tipo.MonedaDestino,
            Valor = tipo.Valor,
            Fecha = tipo.Fecha,
            Activo = tipo.Activo
        };

        return View(model);
    }

    /// <summary>
    /// Muestra el formulario de edición con los datos actuales del tipo de cambio.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(
        int id, CancellationToken cancellationToken = default)
    {
        var tipo = await _service.ObtenerPorIdAsync(id, cancellationToken);
        if (tipo is null) return NotFound();

        var model = new EditarTipoCambioViewModel
        {
            Id = tipo.Id,
            Valor = tipo.Valor,
            Fecha = tipo.Fecha.ToDateTime(TimeOnly.MinValue)
        };

        return View(model);
    }

    /// <summary>
    /// Procesa la actualización de un tipo de cambio existente.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id, EditarTipoCambioViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        try
        {
            var resultado = await _service.ActualizarAsync(
                id, model.Valor,
                DateOnly.FromDateTime(model.Fecha!.Value),
                cancellationToken);

            if (resultado is null) return NotFound();
        }
        catch (DomainException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }

        TempData["MensajeExito"] = "El tipo de cambio se actualizó correctamente.";
        return RedirectToAction(nameof(Details), new { id });
    }

    /// <summary>
    /// Muestra la confirmación de desactivación de un tipo de cambio.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Delete(
        int id, CancellationToken cancellationToken = default)
    {
        var tipo = await _service.ObtenerPorIdAsync(id, cancellationToken);
        if (tipo is null) return NotFound();

        var model = new EliminarTipoCambioViewModel
        {
            Id = tipo.Id,
            MonedaOrigen = tipo.MonedaOrigen,
            MonedaDestino = tipo.MonedaDestino,
            Valor = tipo.Valor,
            Fecha = tipo.Fecha,
            Activo = tipo.Activo
        };

        return View(model);
    }

    /// <summary>
    /// Confirma la desactivación de un tipo de cambio y redirige al listado.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        int id, CancellationToken cancellationToken)
    {
        var desactivado = await _service.EliminarAsync(id, cancellationToken);
        if (!desactivado) return NotFound();

        TempData["MensajeExito"] = "El tipo de cambio fue desactivado.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Activa un tipo de cambio previamente desactivado.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activar(
        int id, CancellationToken cancellationToken = default)
    {
        var resultado = await _service.ActivarAsync(id, cancellationToken);
        if (resultado is null) return NotFound();

        TempData["MensajeExito"] = "El tipo de cambio se activó correctamente.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
