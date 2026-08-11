using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Web.Models.Proveedores;

using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

public sealed class ProveedoresController : Controller
{
    private readonly CrearProveedorService _service;

    public ProveedoresController(CrearProveedorService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CrearProveedorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CrearProveedorViewModel model,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _service.CrearAsync(
                new CrearProveedorRequest(model.Nombre),
                cancellationToken);
        }
        catch (ProveedorDuplicadoException)
        {
            ModelState.AddModelError(
                nameof(CrearProveedorViewModel.Nombre),
                "Ya existe un proveedor con ese nombre.");

            return View(model);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(
                nameof(CrearProveedorViewModel.Nombre),
                exception.Message);

            return View(model);
        }

        TempData["MensajeExito"] = "El proveedor se registró correctamente.";
        return RedirectToAction(nameof(Create));
    }
}
