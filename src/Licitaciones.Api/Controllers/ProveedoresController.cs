using Licitaciones.Application.Common;
using Licitaciones.Application.Proveedores;
using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Application.Proveedores.Editar;
using Licitaciones.Application.Proveedores.Eliminar;

using Microsoft.AspNetCore.Mvc;

using ApplicationRequest = Licitaciones.Application.Proveedores.Crear.CrearProveedorRequest;
using HttpEditRequest = Licitaciones.Api.Contracts.Proveedores.EditarProveedorRequest;
using HttpRequest = Licitaciones.Api.Contracts.Proveedores.CrearProveedorRequest;

namespace Licitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/proveedores")]
public sealed class ProveedoresController : ControllerBase
{
    private readonly CrearProveedorService _crearService;
    private readonly ConsultarProveedorService? _consultarService;
    private readonly EditarProveedorService? _editarService;
    private readonly DarBajaProveedorService? _darBajaService;

    public ProveedoresController(
        CrearProveedorService crearService,
        ConsultarProveedorService? consultarService = null,
        EditarProveedorService? editarService = null,
        DarBajaProveedorService? darBajaService = null)
    {
        _crearService = crearService;
        _consultarService = consultarService;
        _editarService = editarService;
        _darBajaService = darBajaService;
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _darBajaService!.DarDeBajaAsync(id, cancellationToken);
            return NoContent();
        }
        catch (Licitaciones.Application.Proveedores.Eliminar.ProveedorNoEncontradoException exception)
        {
            return NotFound(CrearProblema(404, "Proveedor no encontrado", exception.Message));
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<ProveedorDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProveedorDto>> Editar(
        Guid id,
        HttpEditRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var proveedor = await _editarService!.EditarAsync(
                id,
                new Licitaciones.Application.Proveedores.Editar.EditarProveedorRequest(
                    request.Nombre, request.Version),
                cancellationToken);
            return Ok(proveedor);
        }
        catch (Licitaciones.Application.Proveedores.Editar.ProveedorNoEncontradoException exception)
        {
            return NotFound(CrearProblema(404, "Proveedor no encontrado", exception.Message));
        }
        catch (ProveedorConcurrenciaException exception)
        {
            return Conflict(CrearProblema(409, "Conflicto de actualización", exception.Message));
        }
        catch (Licitaciones.Application.Proveedores.Editar.ProveedorDuplicadoException exception)
        {
            return Conflict(CrearProblema(409, "Proveedor duplicado", exception.Message));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(CrearProblema(400, "Nombre de proveedor inválido", exception.Message));
        }
    }

    private ProblemDetails CrearProblema(int status, string title, string detail) => new()
    {
        Status = status,
        Title = title,
        Detail = detail,
        Type = $"https://httpstatuses.com/{status}",
        Instance = HttpContext.Request.Path
    };

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

    [HttpGet("historico")]
    [ProducesResponseType<PaginaResultado<ProveedorHistoricoDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginaResultado<ProveedorHistoricoDto>>> ListarHistorico(
        int pagina = 1,
        int tamanoPagina = 20,
        string? nombre = null,
        ProveedorOrden ordenarPor = ProveedorOrden.Nombre,
        bool descendente = false,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _consultarService!.ListarHistoricoAsync(
            new ConsultarProveedoresRequest(
                pagina, tamanoPagina, nombre, ordenarPor, descendente),
            cancellationToken);

        return Ok(resultado);
    }

    [HttpGet("historico/{id:guid}")]
    [ProducesResponseType<ProveedorHistoricoDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProveedorHistoricoDto>> ObtenerHistoricoPorId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var proveedor = await _consultarService!.ObtenerHistoricoPorIdAsync(
            id,
            cancellationToken);
        return proveedor is null ? NotFound() : Ok(proveedor);
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
        catch (Licitaciones.Application.Proveedores.Crear.ProveedorDuplicadoException)
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
