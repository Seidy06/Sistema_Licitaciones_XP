using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.Licitaciones.Cerrar;
using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Application.Licitaciones.Crear;
using Licitaciones.Application.Licitaciones.Editar;
using Licitaciones.Application.Licitaciones.Publicar;
using Licitaciones.Domain.Common;

using Microsoft.AspNetCore.Mvc;

using ApplicationRequest = Licitaciones.Application.Licitaciones.Crear.CrearLicitacionRequest;
using EditarApplicationRequest = Licitaciones.Application.Licitaciones.Editar.EditarLicitacionRequest;
using EditarHttpRequest = Licitaciones.Api.Contracts.Licitaciones.EditarLicitacionRequest;
using HttpRequest = Licitaciones.Api.Contracts.Licitaciones.CrearLicitacionRequest;

namespace Licitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/licitaciones")]
public sealed class LicitacionesController : ControllerBase
{
    private readonly CrearLicitacionService _crearService;
    private readonly ConsultarLicitacionService _consultarService;
    private readonly EditarLicitacionService _editarService;
    private readonly PublicarLicitacionService _publicarService;
    private readonly CerrarLicitacionService _cerrarService;
    private readonly IClock _clock;

    public LicitacionesController(
        CrearLicitacionService crearService,
        ConsultarLicitacionService consultarService,
        EditarLicitacionService editarService,
        PublicarLicitacionService publicarService,
        CerrarLicitacionService cerrarService,
        IClock clock)
    {
        _crearService = crearService;
        _consultarService = consultarService;
        _editarService = editarService;
        _publicarService = publicarService;
        _cerrarService = cerrarService;
        _clock = clock;
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LicitacionDto>> Editar(
        Guid id,
        EditarHttpRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return Ok(await _editarService.EditarAsync(new EditarApplicationRequest(
                id, request.Codigo, request.Titulo, request.Presupuesto, request.FechaCierre),
                cancellationToken));
        }
        catch (LicitacionNoEncontradaException exception)
        {
            return NotFound(CrearProblema(404, "LicitaciÃ³n no encontrada", exception.Message));
        }
        catch (DomainException exception)
        {
            return BadRequest(CrearProblema(400, "EdiciÃ³n invÃ¡lida", exception.Message));
        }
    }

    [HttpPost("{id:guid}/publicar")]
    public Task<ActionResult<LicitacionDto>> Publicar(Guid id, CancellationToken cancellationToken) =>
        CambiarEstadoAsync(id, true, cancellationToken);

    [HttpPost("{id:guid}/cerrar")]
    public Task<ActionResult<LicitacionDto>> Cerrar(Guid id, CancellationToken cancellationToken) =>
        CambiarEstadoAsync(id, false, cancellationToken);

    private async Task<ActionResult<LicitacionDto>> CambiarEstadoAsync(
        Guid id,
        bool publicar,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = publicar
                ? await _publicarService.PublicarAsync(id, cancellationToken)
                : await _cerrarService.CerrarAsync(id, cancellationToken);
            return Ok(resultado);
        }
        catch (LicitacionNoEncontradaException exception)
        {
            return NotFound(CrearProblema(404, "LicitaciÃ³n no encontrada", exception.Message));
        }
        catch (DomainException exception)
        {
            return BadRequest(CrearProblema(400, "TransiciÃ³n invÃ¡lida", exception.Message));
        }
    }

    private ProblemDetails CrearProblema(int status, string title, string detail) => new()
    {
        Status = status,
        Title = title,
        Detail = detail,
        Instance = HttpContext.Request.Path
    };

    [HttpGet]
    [ProducesResponseType<PaginaLicitaciones>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginaLicitaciones>> Listar(
        [FromQuery] ConsultarLicitacionesRequest consulta,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _consultarService.ListarAsync(
                consulta, _clock, cancellationToken);
            return Ok(resultado);
        }
        catch (DomainException exception)
        {
            return BadRequest(CrearProblema(400, "Consulta invÃ¡lida", exception.Message));
        }
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
