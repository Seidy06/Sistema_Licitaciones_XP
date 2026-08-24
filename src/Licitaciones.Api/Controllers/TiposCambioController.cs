using Licitaciones.Api.Contracts.TiposCambio;
using Licitaciones.Api.Infraestructura;
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
        return tipoCambio is null
            ? CrearProblema(
                StatusCodes.Status404NotFound,
                "Tipo de cambio no encontrado",
                "No existe un tipo de cambio activo.",
                "tipo_cambio_no_encontrado")
            : Ok(tipoCambio);
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
            return CrearProblema(
                StatusCodes.Status400BadRequest,
                "Tipo de cambio inválido",
                exception.Message,
                "tipo_cambio_invalido");
        }
    }

    private ObjectResult CrearProblema(int estado, string titulo, string detalle, string codigoError) =>
        RespuestaProblema.Crear(HttpContext, estado, titulo, detalle, codigoError);
}
