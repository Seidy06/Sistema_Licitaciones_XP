using Licitaciones.Api.Contracts.Aprobaciones;
using Licitaciones.Api.Infraestructura;
using Licitaciones.Application.Aprobaciones;
using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Domain.Common;

using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/niveles-aprobacion")]
public sealed class NivelesAprobacionController : ControllerBase
{
    private readonly AdministrarNivelesAprobacionService _administrar;
    private readonly ResolverNivelAprobacionService _resolver;

    public NivelesAprobacionController(
        AdministrarNivelesAprobacionService administrar,
        ResolverNivelAprobacionService resolver)
    {
        _administrar = administrar;
        _resolver = resolver;
    }

    [HttpGet("resolver")]
    [ProducesResponseType<LicitacionNivelAprobacionDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LicitacionNivelAprobacionDto>> Resolver(
        decimal monto,
        CancellationToken cancellationToken)
    {
        var nivel = await _resolver.ResolverAsync(monto, cancellationToken);
        return nivel is null
            ? CrearProblema(
                StatusCodes.Status404NotFound,
                "Nivel de aprobación no encontrado",
                "No existe un nivel de aprobación configurado para el monto indicado.",
                "nivel_aprobacion_no_encontrado")
            : Ok(nivel);
    }

    [HttpPost]
    public async Task<ActionResult<LicitacionNivelAprobacionDto>> Crear(
        GuardarNivelAprobacionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var nivel = await _administrar.CrearAsync(
                request.Nombre,
                request.MontoMinimo,
                request.MontoMaximo,
                cancellationToken);
            return Created($"/api/v1/niveles-aprobacion/{nivel.Id}", nivel);
        }
        catch (NivelAprobacionConflictoException exception)
        {
            return CrearProblema(
                StatusCodes.Status409Conflict,
                "Rango de aprobación en conflicto",
                exception.Message,
                "nivel_aprobacion_conflicto");
        }
        catch (DomainException exception)
        {
            return CrearProblema(
                StatusCodes.Status400BadRequest,
                "Nivel de aprobación inválido",
                exception.Message,
                "nivel_aprobacion_invalido");
        }
    }

    private ObjectResult CrearProblema(int estado, string titulo, string detalle, string codigoError) =>
        RespuestaProblema.Crear(HttpContext, estado, titulo, detalle, codigoError);
}
