using Licitaciones.Api.Infraestructura;
using Licitaciones.Application.Ofertas;
using Licitaciones.Application.Ofertas.Consultar;
using Licitaciones.Application.Ofertas.Crear;
using Licitaciones.Application.Ofertas.Editar;
using Licitaciones.Application.Ofertas.Eliminar;
using Licitaciones.Domain.Common;

using Microsoft.AspNetCore.Mvc;

using CrearHttpRequest = Licitaciones.Api.Contracts.Ofertas.CrearOfertaRequest;
using CrearRequest = Licitaciones.Application.Ofertas.Crear.CrearOfertaRequest;

namespace Licitaciones.Api.Controllers;

/// <summary>
/// API REST para gestionar ofertas de licitaciones.
/// </summary>
[ApiController]
[Route("api/v1/ofertas")]
public sealed class OfertasController : ControllerBase
{
    private readonly CrearOfertaService _crearOfertaService;
    private readonly EditarOfertaService _editarOfertaService;
    private readonly EliminarOfertaService _eliminarOfertaService;
    private readonly ConsultarOfertaService _consultarOfertaService;

    public OfertasController(
        CrearOfertaService crearOfertaService,
        EditarOfertaService editarOfertaService,
        EliminarOfertaService eliminarOfertaService,
        ConsultarOfertaService consultarOfertaService)
    {
        _crearOfertaService = crearOfertaService;
        _editarOfertaService = editarOfertaService;
        _eliminarOfertaService = eliminarOfertaService;
        _consultarOfertaService = consultarOfertaService;
    }

    /// <summary>
    /// Lista ofertas de una licitación con paginación y filtros opcionales.
    /// </summary>
    [HttpGet]
    [ProducesResponseType<PaginaOfertas>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginaOfertas>> Listar(
        [FromQuery] Guid licitacionId,
        [FromQuery] string moneda = "CRC",
        [FromQuery] string? proveedor = null,
        [FromQuery] string ordenarPor = "monto",
        [FromQuery] bool descendente = false,
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await _consultarOfertaService.ListarAsync(
                new ConsultarOfertasRequest(
                    licitacionId, moneda, proveedor, ordenarPor,
                    descendente, pagina, tamanoPagina),
                cancellationToken));
        }
        catch (DomainException exception)
        {
            return CrearProblema(
                StatusCodes.Status400BadRequest,
                "Consulta de ofertas inválida",
                exception.Message,
                "consulta_ofertas_invalida");
        }
    }

    /// <summary>
    /// Obtiene el detalle de una oferta por su identificador.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType<OfertaConsultaDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OfertaConsultaDto>> Obtener(
        Guid id,
        [FromQuery] string moneda = "CRC",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var oferta = await _consultarOfertaService.ObtenerAsync(
                id, moneda, cancellationToken);
            return oferta is null
                ? CrearProblema(
                    StatusCodes.Status404NotFound,
                    "Oferta no encontrada",
                    "La oferta solicitada no existe.",
                    "oferta_no_encontrada")
                : Ok(oferta);
        }
        catch (DomainException exception)
        {
            return CrearProblema(
                StatusCodes.Status400BadRequest,
                "Consulta de oferta inválida",
                exception.Message,
                "consulta_oferta_invalida");
        }
    }

    /// <summary>
    /// Crea una nueva oferta para una licitación.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<OfertaDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<OfertaDto>> Crear(
        CrearHttpRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var oferta = await _crearOfertaService.CrearAsync(
                new CrearRequest(
                    request.LicitacionId, request.ProveedorId, request.Monto),
                cancellationToken);

            return Created($"/api/v1/ofertas/{oferta.Id}", oferta);
        }
        catch (OfertaDuplicadaException exception)
        {
            return CrearProblema(
                StatusCodes.Status409Conflict,
                "Oferta duplicada",
                exception.Message,
                "oferta_duplicada");
        }
        catch (DomainException exception)
            when (exception.Code == OfertaErrorCodes.NoProcesable)
        {
            return CrearProblema(
                StatusCodes.Status422UnprocessableEntity,
                "Oferta rechazada",
                exception.Message,
                "oferta_no_procesable");
        }
        catch (DomainException exception)
        {
            return CrearProblema(
                StatusCodes.Status400BadRequest,
                "Solicitud inválida",
                exception.Message,
                "solicitud_invalida");
        }
    }

    /// <summary>
    /// Actualiza el monto de una oferta existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Editar(
        Guid id,
        [FromBody] EditarOfertaRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var oferta = await _editarOfertaService.EditarAsync(
                new EditarOfertaRequest(id, request.Monto), cancellationToken);
            return Ok(oferta);
        }
        catch (DomainException exception)
            when (exception.Code == OfertaErrorCodes.NoProcesable)
        {
            return CrearProblema(
                StatusCodes.Status422UnprocessableEntity,
                "Oferta inalterable",
                exception.Message,
                "oferta_no_procesable");
        }
        catch (DomainException exception)
        {
            return CrearProblema(
                StatusCodes.Status400BadRequest,
                "Solicitud inválida",
                exception.Message,
                "solicitud_invalida");
        }
    }

    /// <summary>
    /// Elimina una oferta del sistema.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Eliminar(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var eliminada = await _eliminarOfertaService.EliminarAsync(id, cancellationToken);
            return eliminada ? NoContent() : NotFound();
        }
        catch (DomainException exception)
            when (exception.Code == OfertaErrorCodes.NoProcesable)
        {
            return CrearProblema(
                StatusCodes.Status422UnprocessableEntity,
                "Oferta inalterable",
                exception.Message,
                "oferta_no_procesable");
        }
        catch (DomainException exception)
        {
            return CrearProblema(
                StatusCodes.Status400BadRequest,
                "Solicitud inválida",
                exception.Message,
                "solicitud_invalida");
        }
    }

    private ObjectResult CrearProblema(int estado, string titulo, string detalle, string codigoError) =>
        RespuestaProblema.Crear(HttpContext, estado, titulo, detalle, codigoError);
}
