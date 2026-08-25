using Licitaciones.Api.Infraestructura;
using Licitaciones.Application.Common;
using Licitaciones.Application.Ofertas.Consultar;
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
    private readonly ConsultarOfertaService? _consultarOfertaService;

    public ProveedoresController(
        CrearProveedorService crearService,
        ConsultarProveedorService? consultarService = null,
        EditarProveedorService? editarService = null,
        DarBajaProveedorService? darBajaService = null,
        ConsultarOfertaService? consultarOfertaService = null)
    {
        _crearService = crearService;
        _consultarService = consultarService;
        _editarService = editarService;
        _darBajaService = darBajaService;
        _consultarOfertaService = consultarOfertaService;
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
            return CrearProblema(
                StatusCodes.Status404NotFound,
                "Proveedor no encontrado",
                exception.Message,
                "proveedor_no_encontrado");
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
            return CrearProblema(
                StatusCodes.Status404NotFound,
                "Proveedor no encontrado",
                exception.Message,
                "proveedor_no_encontrado");
        }
        catch (ProveedorConcurrenciaException exception)
        {
            return CrearProblema(
                StatusCodes.Status409Conflict,
                "Conflicto de actualización",
                exception.Message,
                "conflicto_actualizacion");
        }
        catch (Licitaciones.Application.Proveedores.Editar.ProveedorDuplicadoException exception)
        {
            return CrearProblema(
                StatusCodes.Status409Conflict,
                "Proveedor duplicado",
                exception.Message,
                "proveedor_duplicado");
        }
        catch (ArgumentException exception)
        {
            return CrearProblema(
                StatusCodes.Status400BadRequest,
                "Nombre de proveedor inválido",
                exception.Message,
                "nombre_proveedor_invalido");
        }
    }

    private ObjectResult CrearProblema(int estado, string titulo, string detalle, string codigoError) =>
        RespuestaProblema.Crear(HttpContext, estado, titulo, detalle, codigoError);

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
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProveedorHistoricoDto>> ObtenerHistoricoPorId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var proveedor = await _consultarService!.ObtenerHistoricoPorIdAsync(
            id,
            cancellationToken);
        return proveedor is null
            ? CrearProblema(
                StatusCodes.Status404NotFound,
                "Proveedor no encontrado",
                "El proveedor solicitado no existe.",
                "proveedor_no_encontrado")
            : Ok(proveedor);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<ProveedorDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProveedorDto>> ObtenerPorId(
        Guid id,
        CancellationToken cancellationToken)
    {
        var proveedor = await _consultarService!.ObtenerPorIdAsync(id, cancellationToken);
        return proveedor is null
            ? CrearProblema(
                StatusCodes.Status404NotFound,
                "Proveedor no encontrado",
                "El proveedor solicitado no existe.",
                "proveedor_no_encontrado")
            : Ok(proveedor);
    }

    [HttpGet("{id:guid}/ofertas")]
    [ProducesResponseType<PaginaOfertas>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginaOfertas>> Ofertas(
        Guid id,
        [FromQuery] string moneda = "CRC",
        [FromQuery] string? licitacionCodigo = null,
        [FromQuery] string ordenarPor = "fecharegistro",
        [FromQuery] bool descendente = false,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var proveedor = await _consultarService!.ObtenerPorIdAsync(id, cancellationToken);
            if (proveedor is null)
            {
                return CrearProblema(
                    StatusCodes.Status404NotFound,
                    "Proveedor no encontrado",
                    "El proveedor solicitado no existe.",
                    "proveedor_no_encontrado");
            }

            return Ok(await _consultarOfertaService!.ListarPorProveedorAsync(
                id, moneda, licitacionCodigo, ordenarPor,
                descendente, pagina, tamanoPagina, cancellationToken));
        }
        catch (Licitaciones.Domain.Common.DomainException exception)
        {
            return CrearProblema(
                StatusCodes.Status400BadRequest,
                "Consulta de ofertas inválida",
                exception.Message,
                "consulta_ofertas_invalida");
        }
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
            return CrearProblema(
                StatusCodes.Status409Conflict,
                "Proveedor duplicado",
                "Ya existe un proveedor con el mismo nombre.",
                "proveedor_duplicado");
        }
        catch (ArgumentException exception)
        {
            return CrearProblema(
                StatusCodes.Status400BadRequest,
                "Nombre de proveedor inválido",
                exception.Message,
                "nombre_proveedor_invalido");
        }
    }
}
