using Licitaciones.Application.Proveedores;
using Licitaciones.Application.Proveedores.Crear;

using Microsoft.AspNetCore.Mvc;

using ApplicationRequest = Licitaciones.Application.Proveedores.Crear.CrearProveedorRequest;
using HttpRequest = Licitaciones.Api.Contracts.Proveedores.CrearProveedorRequest;

namespace Licitaciones.Api.Controllers;

[ApiController]
[Route("api/v1/proveedores")]
public sealed class ProveedoresController : ControllerBase
{
    private readonly CrearProveedorService _service;

    public ProveedoresController(CrearProveedorService service)
    {
        _service = service;
    }

    [HttpPost]
    [ProducesResponseType<ProveedorDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProveedorDto>> Crear(
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var proveedor = await _service.CrearAsync(
                new ApplicationRequest(request.Nombre),
                cancellationToken);

            return Created($"/api/v1/proveedores/{proveedor.Id}", proveedor);
        }
        catch (ProveedorDuplicadoException)
        {
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Proveedor duplicado",
                Detail = "Ya existe un proveedor con el mismo nombre.",
                Type = "https://httpstatuses.com/409",
                Instance = HttpContext.Request.Path
            });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Nombre de proveedor inválido",
                Detail = exception.Message,
                Type = "https://httpstatuses.com/400",
                Instance = HttpContext.Request.Path
            });
        }
    }
}
