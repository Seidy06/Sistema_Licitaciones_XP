using Licitaciones.Application.Ofertas;
using Licitaciones.Application.Ofertas.Crear;
using Licitaciones.Application.Ofertas.Consultar;
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
    [ProducesResponseType<IReadOnlyList<OfertaConsultaDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<OfertaConsultaDto>>> Listar(
        [FromQuery] Guid licitacionId,
        [FromQuery] string moneda = "CRC",
        CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await _consultarOfertaService.ListarAsync(
                licitacionId, moneda, cancellationToken));
        }
        catch (DomainException exception)
        {
            return BadRequest(CrearProblema(
                StatusCodes.Status400BadRequest,
                "Consulta de ofertas inválida",
                exception.Message));
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
            return oferta is null ? NotFound() : Ok(oferta);
        }
        catch (DomainException exception)
        {
            return BadRequest(CrearProblema(
                StatusCodes.Status400BadRequest,
                "Consulta de oferta inválida",
                exception.Message));
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
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Oferta duplicada",
                Detail = exception.Message,
                Instance = HttpContext.Request.Path
            });
        }
        catch (DomainException exception)
            when (exception.Code == OfertaErrorCodes.NoProcesable)
        {
            return UnprocessableEntity(CrearProblema(
                StatusCodes.Status422UnprocessableEntity,
                "Oferta rechazada",
                exception.Message));
        }
        catch (DomainException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Oferta rechazada",
                Detail = exception.Message,
                Instance = HttpContext.Request.Path
            });
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
            return UnprocessableEntity(CrearProblema(
                StatusCodes.Status422UnprocessableEntity,
                "Oferta inalterable",
                exception.Message));
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
}
