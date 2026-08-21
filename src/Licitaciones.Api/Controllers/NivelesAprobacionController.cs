using Licitaciones.Api.Contracts.Aprobaciones;
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
    public async Task<ActionResult<LicitacionNivelAprobacionDto>> Resolver(
        decimal monto,
        CancellationToken cancellationToken)
    {
        var nivel = await _resolver.ResolverNivelAprobacion(monto, cancellationToken);
        return nivel is null ? NotFound() : Ok(nivel);
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
            return Conflict(Problema(409, "Rango de aprobación en conflicto", exception.Message));
        }
        catch (DomainException exception)
        {
            return BadRequest(Problema(400, "Nivel de aprobación inválido", exception.Message));
        }
    }

    private ProblemDetails Problema(int status, string title, string detail) => new()
    {
        Status = status,
        Title = title,
        Detail = detail,
        Type = $"https://httpstatuses.com/{status}",
        Instance = HttpContext.Request.Path
    };
}
