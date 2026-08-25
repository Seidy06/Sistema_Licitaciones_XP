using Licitaciones.Api.Contracts.Aprobaciones;
using Licitaciones.Api.Infraestructura;
using Licitaciones.Application.Aprobaciones;
using Licitaciones.Application.Common;
using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Domain.Common;

using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Api.Controllers;

/// <summary>
/// API REST para gestionar niveles de aprobación de licitaciones.
/// </summary>
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

    /// <summary>
    /// Lista niveles de aprobación con paginación y filtros.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<PaginaResultado<NivelAprobacionResumenDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginaResultado<NivelAprobacionResumenDto>>> Listar(
        [FromQuery] NivelesAprobacionConsultaRequest consulta,
        CancellationToken cancellationToken)
    {
        try
        {
            var resultado = await _administrar.ListarAsync(consulta, cancellationToken);
            return Ok(resultado);
        }
        catch (DomainException exception)
        {
            return CrearProblema(
                StatusCodes.Status400BadRequest,
                "Consulta inválida",
                exception.Message,
                "consulta_niveles_aprobacion_invalida");
        }
    }

    /// <summary>
    /// Obtiene un nivel de aprobación por su identificador.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<NivelAprobacionResumenDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NivelAprobacionResumenDto>> Obtener(
        int id,
        CancellationToken cancellationToken)
    {
        var nivel = await _administrar.ObtenerPorIdAsync(id, cancellationToken);
        return nivel is null
            ? CrearProblema(
                StatusCodes.Status404NotFound,
                "Nivel de aprobación no encontrado",
                "El nivel de aprobación solicitado no existe.",
                "nivel_aprobacion_no_encontrado")
            : Ok(nivel);
    }

    /// <summary>
    /// Resuelve el nivel de aprobación correspondiente a un monto dado.
    /// </summary>
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

    /// <summary>
    /// Crea un nuevo nivel de aprobación.
    /// </summary>
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

    /// <summary>
    /// Actualiza un nivel de aprobación existente.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType<NivelAprobacionResumenDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<NivelAprobacionResumenDto>> Actualizar(
        int id,
        GuardarNivelAprobacionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var nivel = await _administrar.ActualizarAsync(
                id,
                request.Nombre,
                request.MontoMinimo,
                request.MontoMaximo,
                cancellationToken);

            return nivel is null
                ? CrearProblema(
                    StatusCodes.Status404NotFound,
                    "Nivel de aprobación no encontrado",
                    "El nivel de aprobación solicitado no existe.",
                    "nivel_aprobacion_no_encontrado")
                : Ok(nivel);
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

    /// <summary>
    /// Desactiva un nivel de aprobación existente.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(
        int id,
        CancellationToken cancellationToken)
    {
        var eliminado = await _administrar.DesactivarAsync(id, cancellationToken);
        return eliminado
            ? NoContent()
            : CrearProblema(
                StatusCodes.Status404NotFound,
                "Nivel de aprobación no encontrado",
                "El nivel de aprobación solicitado no existe o ya está desactivado.",
                "nivel_aprobacion_no_encontrado");
    }

    private ObjectResult CrearProblema(int estado, string titulo, string detalle, string codigoError) =>
        RespuestaProblema.Crear(HttpContext, estado, titulo, detalle, codigoError);
}
