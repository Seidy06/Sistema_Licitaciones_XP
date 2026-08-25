using Licitaciones.Api.Contracts.TiposCambio;
using Licitaciones.Api.Infraestructura;
using Licitaciones.Application.Common;
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

    [HttpGet]
    [ProducesResponseType<PaginaResultado<TipoCambioDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginaResultado<TipoCambioDto>>> Listar(
        string ordenarPor = "fecha",
        bool descendente = false,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var resultado = await _administrar.ListarAsync(
                ordenarPor, descendente, pagina, tamanoPagina, cancellationToken);
            return Ok(resultado);
        }
        catch (DomainException exception)
        {
            return CrearProblema(
                StatusCodes.Status400BadRequest,
                "Consulta inválida",
                exception.Message,
                "consulta_tipos_cambio_invalida");
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<TipoCambioDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TipoCambioDto>> Obtener(
        int id,
        CancellationToken cancellationToken)
    {
        var tipoCambio = await _administrar.ObtenerPorIdAsync(id, cancellationToken);
        return tipoCambio is null
            ? CrearProblema(
                StatusCodes.Status404NotFound,
                "Tipo de cambio no encontrado",
                "El tipo de cambio solicitado no existe.",
                "tipo_cambio_no_encontrado")
            : Ok(tipoCambio);
    }

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

    [HttpPut("{id:int}")]
    [ProducesResponseType<TipoCambioDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TipoCambioDto>> Actualizar(
        int id,
        GuardarTipoCambioRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var tipoCambio = await _administrar.ActualizarAsync(
                id,
                request.Valor,
                request.Fecha,
                cancellationToken);

            return tipoCambio is null
                ? CrearProblema(
                    StatusCodes.Status404NotFound,
                    "Tipo de cambio no encontrado",
                    "El tipo de cambio solicitado no existe.",
                    "tipo_cambio_no_encontrado")
                : Ok(tipoCambio);
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

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(
        int id,
        CancellationToken cancellationToken)
    {
        var eliminado = await _administrar.EliminarAsync(id, cancellationToken);
        return eliminado
            ? NoContent()
            : CrearProblema(
                StatusCodes.Status404NotFound,
                "Tipo de cambio no encontrado",
                "El tipo de cambio solicitado no existe.",
                "tipo_cambio_no_encontrado");
    }

    [HttpPatch("{id:int}/activar")]
    [ProducesResponseType<TipoCambioDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TipoCambioDto>> Activar(
        int id,
        CancellationToken cancellationToken)
    {
        var tipoCambio = await _administrar.ActivarAsync(id, cancellationToken);
        return tipoCambio is null
            ? CrearProblema(
                StatusCodes.Status404NotFound,
                "Tipo de cambio no encontrado",
                "El tipo de cambio solicitado no existe.",
                "tipo_cambio_no_encontrado")
            : Ok(tipoCambio);
    }

    private ObjectResult CrearProblema(int estado, string titulo, string detalle, string codigoError) =>
        RespuestaProblema.Crear(HttpContext, estado, titulo, detalle, codigoError);
}
