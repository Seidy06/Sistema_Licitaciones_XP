using Licitaciones.Application.Licitaciones.Crear;
using Licitaciones.Web.Models.Licitaciones;

using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

public sealed class LicitacionesController : Controller
{
    [HttpGet]
    public IActionResult Create() => View(new CrearLicitacionViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CrearLicitacionViewModel model,
        [FromServices] CrearLicitacionService service,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await service.CrearAsync(
                new CrearLicitacionRequest(
                    model.Codigo,
                    model.Titulo,
                    model.Presupuesto,
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
}
