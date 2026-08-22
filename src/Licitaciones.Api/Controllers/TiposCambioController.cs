using Licitaciones.Api.Contracts.TiposCambio;
using Licitaciones.Application.TiposCambio;
using Licitaciones.Domain.Common;

using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/tipos-cambio")]
public sealed class TiposCambioController : ControllerBase
{
    private readonly AdministrarTipoCambioService _administrar;

    public TiposCambioController(AdministrarTipoCambioService administrar) =>
        _administrar = administrar;

    [HttpGet("activo")]
    [ProducesResponseType<TipoCambioDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TipoCambioDto>> ObtenerActivo(
        CancellationToken cancellationToken)
    {
        var tipoCambio = await _administrar.ObtenerActivoAsync(cancellationToken);
        return tipoCambio is null ? NotFound() : Ok(tipoCambio);
    }

    [HttpPost]
    [ProducesResponseType<TipoCambioDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TipoCambioDto>> Guardar(
        GuardarTipoCambioRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var tipoCambio = await _administrar.GuardarAsync(
                request.Valor,
                request.Fecha,
                cancellationToken);
            return Created($"/api/v1/tipos-cambio/{tipoCambio.Id}", tipoCambio);
        }
        catch (DomainException exception)
        {
            return BadRequest(CrearProblema(
                StatusCodes.Status400BadRequest,
                "Tipo de cambio inválido",
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
