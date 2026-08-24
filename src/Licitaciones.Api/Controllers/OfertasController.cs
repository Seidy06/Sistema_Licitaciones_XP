using Licitaciones.Api.Infraestructura;
using Licitaciones.Application.Ofertas;
using Licitaciones.Application.Ofertas.Consultar;
using Licitaciones.Application.Ofertas.Crear;
using Licitaciones.Application.Ofertas.Proteger;
using Licitaciones.Domain.Common;

using Microsoft.AspNetCore.Mvc;

using ApplicationRequest = Licitaciones.Application.Ofertas.Crear.CrearOfertaRequest;
using HttpRequest = Licitaciones.Api.Contracts.Ofertas.CrearOfertaRequest;

namespace Licitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/ofertas")]
public sealed class OfertasController : ControllerBase
{
    private readonly CrearOfertaService _crearOfertaService;
    private readonly ProtegerOfertaService _protegerOfertaService;
    private readonly ConsultarOfertaService _consultarOfertaService;

    public OfertasController(
        CrearOfertaService crearOfertaService,
        ProtegerOfertaService protegerOfertaService,
        ConsultarOfertaService consultarOfertaService)
    {
        _crearOfertaService = crearOfertaService;
        _protegerOfertaService = protegerOfertaService;
        _consultarOfertaService = consultarOfertaService;
    }

    [HttpGet]
    [ProducesResponseType<PaginaOfertas>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginaOfertas>> Listar(
        [FromQuery] Guid licitacionId,
        [FromQuery] string moneda = "CRC",
        [FromQuery] string? proveedor = null,
        [FromQuery] string ordenarPor = "monto",
        [FromQuery] bool descendente = false,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await _consultarOfertaService.ListarAsync(
                new ConsultarOfertasRequest(
                    licitacionId, moneda, proveedor, ordenarPor,
                    descendente, pagina, tamanoPagina),
                cancellationToken));
        }
        catch (DomainException exception)
        {
            return CrearProblema(
                StatusCodes.Status400BadRequest,
                "Consulta de ofertas inválida",
                exception.Message,
                "consulta_ofertas_invalida");
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<OfertaConsultaDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OfertaConsultaDto>> Obtener(
        Guid id,
        [FromQuery] string moneda = "CRC",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var oferta = await _consultarOfertaService.ObtenerAsync(
                id, moneda, cancellationToken);
            return oferta is null
                ? CrearProblema(
                    StatusCodes.Status404NotFound,
                    "Oferta no encontrada",
                    "La oferta solicitada no existe.",
                    "oferta_no_encontrada")
                : Ok(oferta);
        }
        catch (DomainException exception)
        {
            return CrearProblema(
                StatusCodes.Status400BadRequest,
                "Consulta de oferta inválida",
                exception.Message,
                "consulta_oferta_invalida");
        }
    }

    [HttpPost]
    [ProducesResponseType<OfertaDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<OfertaDto>> Crear(
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var oferta = await _crearOfertaService.CrearAsync(
                new ApplicationRequest(
                    request.LicitacionId, request.ProveedorId, request.Monto),
                cancellationToken);

            return Created($"/api/v1/ofertas/{oferta.Id}", oferta);
        }
        catch (OfertaDuplicadaException exception)
        {
            return CrearProblema(
                StatusCodes.Status409Conflict,
                "Oferta duplicada",
                exception.Message,
                "oferta_duplicada");
        }
        catch (DomainException exception)
            when (exception.Code == OfertaErrorCodes.NoProcesable)
        {
            return CrearProblema(
                StatusCodes.Status422UnprocessableEntity,
                "Oferta rechazada",
                exception.Message,
                "oferta_no_procesable");
        }
        catch (DomainException exception)
        {
            return CrearProblema(
                StatusCodes.Status400BadRequest,
                "Solicitud inválida",
                exception.Message,
                "solicitud_invalida");
        }
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> Editar(
        Guid id,
        CancellationToken cancellationToken) =>
        RechazarCambioAsync(id, cancellationToken);

    [HttpDelete("{id:guid}")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public Task<IActionResult> Eliminar(
        Guid id,
        CancellationToken cancellationToken) =>
        RechazarCambioAsync(id, cancellationToken);

    private async Task<IActionResult> RechazarCambioAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            await _protegerOfertaService.RechazarCambioAsync(id, cancellationToken);
            return NoContent();
        }
        catch (DomainException exception)
            when (exception.Code == OfertaErrorCodes.NoProcesable)
        {
            return CrearProblema(
                StatusCodes.Status422UnprocessableEntity,
                "Oferta inalterable",
                exception.Message,
                "oferta_inalterable");
        }
    }

    private ObjectResult CrearProblema(int estado, string titulo, string detalle, string codigoError) =>
        RespuestaProblema.Crear(HttpContext, estado, titulo, detalle, codigoError);
}
