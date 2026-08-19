using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Application.Licitaciones.Crear;
using Licitaciones.Domain.Common;

using Microsoft.AspNetCore.Mvc;

using ApplicationRequest = Licitaciones.Application.Licitaciones.Crear.CrearLicitacionRequest;
using HttpRequest = Licitaciones.Api.Contracts.Licitaciones.CrearLicitacionRequest;

namespace Licitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/licitaciones")]
public sealed class LicitacionesController : ControllerBase
{
    private readonly CrearLicitacionService _crearService;
    private readonly ConsultarLicitacionService _consultarService;
    private readonly IClock _clock;

    public LicitacionesController(
        CrearLicitacionService crearService,
        ConsultarLicitacionService consultarService,
        IClock clock)
    {
        _crearService = crearService;
        _consultarService = consultarService;
        _clock = clock;
    }

    [HttpGet]
    [ProducesResponseType<PaginaLicitaciones>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginaLicitaciones>> Listar(
        CancellationToken cancellationToken)
    {
        var resultado = await _consultarService.ListarAsync(
            new ConsultarLicitacionesRequest(),
            _clock,
            cancellationToken);

        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType<LicitacionDetalleDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LicitacionDetalleDto>> Obtener(
        Guid id,
        CancellationToken cancellationToken)
    {
        var detalle = await _consultarService.ObtenerDetalleAsync(
            id, _clock, cancellationToken);

        if (detalle is null)
        {
            return NotFound();
        }

        return Ok(detalle);
    }

    [HttpPost]
    [ProducesResponseType<LicitacionDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LicitacionDto>> Crear(
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var licitacion = await _crearService.CrearAsync(
                new ApplicationRequest(
                    request.Codigo,
                    request.Titulo,
                    request.Presupuesto,
                    request.FechaCierre),
                cancellationToken);

            return Created($"/api/v1/licitaciones/{licitacion.Id}", licitacion);
        }
        catch (LicitacionDuplicadoException exception)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Licitación duplicada",
                Detail = exception.Message
            });
        }
    }
}
