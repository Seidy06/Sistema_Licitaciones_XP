using Licitaciones.Application.Common;
using Licitaciones.Application.TiposCambio;
using Licitaciones.Domain.Common;
using Licitaciones.Web.Models.TiposCambio;

using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

public sealed class TiposCambioController : Controller
{
    private readonly AdministrarTipoCambioService _service;

    public TiposCambioController(AdministrarTipoCambioService service)
    {
        _service = service;
    }

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

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CrearTipoCambioViewModel());
    }

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
}
