using Licitaciones.Application.Common;
using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Application.Ofertas.Consultar;
using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Application.Proveedores.Editar;
using Licitaciones.Application.Proveedores.Eliminar;
using Licitaciones.Domain.Common;
using Licitaciones.Web.Models.Proveedores;

using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Web.Controllers;

public sealed class ProveedoresController : Controller
{
    private readonly CrearProveedorService _crearService;
    private readonly ConsultarProveedorService _consultarService;
    private readonly EditarProveedorService? _editarService;
    private readonly DarBajaProveedorService? _darBajaService;
    private readonly ConsultarOfertaService _consultarOfertaService;

    public ProveedoresController(
        CrearProveedorService crearService,
        ConsultarProveedorService consultarService,
        EditarProveedorService? editarService = null,
        DarBajaProveedorService? darBajaService = null,
        ConsultarOfertaService? consultarOfertaService = null)
    {
        _crearService = crearService;
        _consultarService = consultarService;
        _editarService = editarService;
        _darBajaService = darBajaService;
        _consultarOfertaService = consultarOfertaService!;
    }

    [HttpGet]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var proveedor = await _consultarService.ObtenerPorIdAsync(id, cancellationToken);
        if (proveedor is null)
        {
            return NotFound();
        }

        return View(new EliminarProveedorViewModel
        {
            Id = proveedor.Id,
            Nombre = proveedor.Nombre
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _darBajaService!.DarDeBajaAsync(id, cancellationToken);
        }
        catch (Licitaciones.Application.Proveedores.Eliminar.ProveedorNoEncontradoException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var proveedor = await _consultarService.ObtenerPorIdAsync(id, cancellationToken);
        if (proveedor is null)
        {
            return NotFound();
        }

        return View(new EditarProveedorViewModel
        {
            Id = proveedor.Id,
            Nombre = proveedor.Nombre,
            Version = proveedor.Version
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        Guid id,
        EditarProveedorViewModel model,
        CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _editarService!.EditarAsync(
                id,
                new EditarProveedorRequest(model.Nombre, model.Version),
                cancellationToken);
        }
        catch (Licitaciones.Application.Proveedores.Editar.ProveedorNoEncontradoException)
        {
            return NotFound();
        }
        catch (ProveedorConcurrenciaException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View(model);
        }
        catch (Licitaciones.Application.Proveedores.Editar.ProveedorDuplicadoException exception)
        {
            ModelState.AddModelError(nameof(model.Nombre), exception.Message);
            return View(model);
        }
        catch (ArgumentException exception)
        {
            ModelState.AddModelError(nameof(model.Nombre), exception.Message);
            return View(model);
        }

        TempData["MensajeExito"] = "El proveedor se actualizó correctamente.";
        return RedirectToAction(nameof(Details), new { id });
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
    public async Task<IActionResult> History(
        int pagina = 1,
        int tamanoPagina = 20,
        string? nombre = null,
        ProveedorOrden ordenarPor = ProveedorOrden.Nombre,
        bool descendente = false,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _consultarService.ListarHistoricoAsync(
            new ConsultarProveedoresRequest(
                pagina, tamanoPagina, nombre, ordenarPor, descendente),
            cancellationToken);

        var model = new PaginaResultado<ProveedorHistoricoResumenViewModel>(
            resultado.Items
                .Select(proveedor => new ProveedorHistoricoResumenViewModel(
                    proveedor.Id,
                    proveedor.Nombre,
                    proveedor.CreatedAt,
                    proveedor.DeletedAt))
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
    public async Task<IActionResult> HistoryDetails(
        Guid id,
        CancellationToken cancellationToken)
    {
        var proveedor = await _consultarService.ObtenerHistoricoPorIdAsync(
            id,
            cancellationToken);
        if (proveedor is null)
        {
            return NotFound();
        }

        return View(new ProveedorHistoricoDetalleViewModel(
            proveedor.Id,
            proveedor.Nombre,
            proveedor.CreatedAt,
            proveedor.UpdatedAt,
            proveedor.DeletedAt));
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
        catch (Licitaciones.Application.Proveedores.Crear.ProveedorDuplicadoException)
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

    [HttpGet]
    public async Task<IActionResult> Ofertas(
        Guid id, CancellationToken cancellationToken = default)
    {
        var proveedor = await _consultarService.ObtenerPorIdAsync(id, cancellationToken);
        if (proveedor is null) return NotFound();

        var licitaciones = await _consultarOfertaService.ListarAsync(
            new ConsultarOfertasRequest(Guid.Empty),
            cancellationToken);

        var todasLasOfertas = new List<OfertaConsultaDto>();
        foreach (var item in licitaciones.Items)
        {
            if (item.ProveedorNombre == proveedor.Nombre)
            {
                todasLasOfertas.Add(item);
            }
        }

        var model = new ProveedorOfertasViewModel
        {
            Proveedor = new ProveedorResumenViewModel(
                proveedor.Id, proveedor.Nombre, proveedor.CreatedAt),
            Ofertas = new PaginaResultado<ProveedorOfertaItemViewModel>(
                todasLasOfertas
                    .Select(o => new ProveedorOfertaItemViewModel(
                        o.Id, "", o.Monto, o.Moneda, o.FechaRegistro))
                    .ToArray(),
                todasLasOfertas.Count, 1, 100),
            Moneda = "CRC"
        };

        return View(model);
    }
}
