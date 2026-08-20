using Licitaciones.Application.Ofertas;
using Licitaciones.Application.Ofertas.Crear;
using Licitaciones.Domain.Common;

using Microsoft.AspNetCore.Mvc;

using ApplicationRequest = Licitaciones.Application.Ofertas.Crear.CrearOfertaRequest;
using HttpRequest = Licitaciones.Api.Contracts.Ofertas.CrearOfertaRequest;

namespace Licitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/ofertas")]
public sealed class OfertasController : ControllerBase
{
    private readonly CrearOfertaService _service;

    public OfertasController(CrearOfertaService service) => _service = service;

    [HttpPost]
    [ProducesResponseType<OfertaDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OfertaDto>> Crear(
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var oferta = await _service.CrearAsync(
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
}
