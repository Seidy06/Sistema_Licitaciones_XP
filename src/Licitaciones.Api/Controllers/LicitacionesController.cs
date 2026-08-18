using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.Licitaciones.Crear;

using Microsoft.AspNetCore.Mvc;

using ApplicationRequest = Licitaciones.Application.Licitaciones.Crear.CrearLicitacionRequest;
using HttpRequest = Licitaciones.Api.Contracts.Licitaciones.CrearLicitacionRequest;

namespace Licitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/licitaciones")]
public sealed class LicitacionesController : ControllerBase
{
    private readonly CrearLicitacionService _service;

    public LicitacionesController(CrearLicitacionService service) => _service = service;

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
            var licitacion = await _service.CrearAsync(
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
