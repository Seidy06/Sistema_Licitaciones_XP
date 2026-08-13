using Licitaciones.Application.Proveedores;
using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Application.Proveedores.Consultar;

using Microsoft.AspNetCore.Mvc;

using ApplicationRequest = Licitaciones.Application.Proveedores.Crear.CrearProveedorRequest;
using HttpRequest = Licitaciones.Api.Contracts.Proveedores.CrearProveedorRequest;

namespace Licitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/proveedores")]
public sealed class ProveedoresController : ControllerBase
{
    private readonly CrearProveedorService _crearService;
    private readonly ConsultarProveedorService? _consultarService;

    public ProveedoresController(CrearProveedorService service)
    {
        _crearService = service;
    }

    public ProveedoresController(
        CrearProveedorService crearService,
        ConsultarProveedorService consultarService)
    {
        _crearService = crearService;
        _consultarService = consultarService;
    }

    [HttpGet]
    [ProducesResponseType<PaginaResultado<ProveedorDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginaResultado<ProveedorDto>>> Listar(
        int pagina = 1,
        int tamanoPagina = 20,
        string? nombre = null,
        ProveedorOrden ordenarPor = ProveedorOrden.Nombre,
        bool descendente = false,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _consultarService!.ListarAsync(
            new ConsultarProveedoresRequest(
                pagina, tamanoPagina, nombre, ordenarPor, descendente),
            cancellationToken);

        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ProveedorDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProveedorDto>> ObtenerPorId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var proveedor = await _consultarService!.ObtenerPorIdAsync(id, cancellationToken);
        return proveedor is null ? NotFound() : Ok(proveedor);
    }

    [HttpPost]
    [ProducesResponseType<ProveedorDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProveedorDto>> Crear(
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var proveedor = await _crearService.CrearAsync(
                new ApplicationRequest(request.Nombre),
                cancellationToken);

            return Created($"/api/v1/proveedores/{proveedor.Id}", proveedor);
        }
        catch (ProveedorDuplicadoException)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Proveedor duplicado",
                Detail = "Ya existe un proveedor con el mismo nombre.",
                Type = "https://httpstatuses.com/409",
                Instance = HttpContext.Request.Path
            });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Nombre de proveedor inválido",
                Detail = exception.Message,
                Type = "https://httpstatuses.com/400",
                Instance = HttpContext.Request.Path
            });
        }
    }
}
