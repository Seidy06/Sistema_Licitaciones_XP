using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Web.Models.Proveedores;

using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

public sealed class ProveedoresController : Controller
{
    private readonly CrearProveedorService _crearService;
    private readonly ConsultarProveedorService _consultarService;

    public ProveedoresController(
        CrearProveedorService crearService,
        ConsultarProveedorService consultarService)
    {
        _crearService = crearService;
        _consultarService = consultarService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        int pagina = 1,
        int tamanoPagina = 20,
        string? nombre = null,
        ProveedorOrden ordenarPor = ProveedorOrden.Nombre,
        bool descendente = false,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _consultarService.ListarAsync(
            new ConsultarProveedoresRequest(
                pagina, tamanoPagina, nombre, ordenarPor, descendente),
            cancellationToken);

        var model = new PaginaResultado<ProveedorResumenViewModel>(
            resultado.Items
                .Select(proveedor => new ProveedorResumenViewModel(
                    proveedor.Id, proveedor.Nombre, proveedor.CreatedAt))
                .ToArray(),
            resultado.Total,
            resultado.Pagina,
            resultado.TamanoPagina);

        ViewData["Nombre"] = nombre;
        ViewData["OrdenarPor"] = ordenarPor;
        ViewData["Descendente"] = descendente;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(
        Guid id,
        CancellationToken cancellationToken)
    {
        var proveedor = await _consultarService.ObtenerPorIdAsync(id, cancellationToken);
        if (proveedor is null)
        {
            return NotFound();
        }

        return View(new ProveedorDetalleViewModel(
            proveedor.Id,
            proveedor.Nombre,
            proveedor.CreatedAt,
            proveedor.UpdatedAt));
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
            await _crearService.CrearAsync(
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
