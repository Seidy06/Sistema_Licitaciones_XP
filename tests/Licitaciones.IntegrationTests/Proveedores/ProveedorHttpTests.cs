using System.ComponentModel.DataAnnotations;
using Licitaciones.Api.Controllers;
using Licitaciones.Application.Proveedores;
using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Domain.Proveedores;
using Licitaciones.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using HttpRequest = Licitaciones.Api.Contracts.Proveedores.CrearProveedorRequest;

namespace Licitaciones.IntegrationTests.Proveedores;

public sealed class ProveedorHttpContractTests
{
    [Fact]
    [Trait("HU", "HU-06-Auditoria")]
    public void Post_DebeAceptarNombreConUnicodeDescompuestoParaNormalizarlo()
    {
        var request = new HttpRequest { Nombre = "Cafe\u0301 Central" };
        var errores = new List<ValidationResult>();

        var esValido = Validator.TryValidateObject(
            request,
            new ValidationContext(request),
            errores,
            validateAllProperties: true);

        Assert.True(
            esValido,
            string.Join(Environment.NewLine, errores.Select(error => error.ErrorMessage)));
    }
}

public sealed class ProveedorHttpTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _database;

    public ProveedorHttpTests(PostgreSqlFixture database)
    {
        _database = database;
    }

    [Fact]
    public async Task Post_DatosValidos_DebeResponderCreated()
    {
        await using var context = _database.CrearContexto();
        var controller = CrearController(context);

        var respuesta = await controller.Crear(
            new HttpRequest { Nombre = $"Proveedor API {Guid.NewGuid():N}" },
            CancellationToken.None);

        var created = Assert.IsType<CreatedResult>(respuesta.Result);
        Assert.Equal(StatusCodes.Status201Created, created.StatusCode);
        var proveedor = Assert.IsType<ProveedorDto>(created.Value);
        Assert.Equal($"/api/v1/proveedores/{proveedor.Id}", created.Location);
    }

    [Fact]
    public async Task Post_NombreDuplicado_DebeResponderConflict()
    {
        var nombre = $"Proveedor duplicado API {Guid.NewGuid():N}";
        await using var context = _database.CrearContexto();
        var controller = CrearController(context);
        await controller.Crear(new HttpRequest { Nombre = nombre }, CancellationToken.None);

        var respuesta = await controller.Crear(
            new HttpRequest { Nombre = $"  {nombre.ToUpperInvariant()}  " },
            CancellationToken.None);

        var conflict = Assert.IsType<ConflictObjectResult>(respuesta.Result);
        Assert.Equal(StatusCodes.Status409Conflict, conflict.StatusCode);
        Assert.IsType<ProblemDetails>(conflict.Value);
    }

    [Fact]
    public async Task Post_NombreInvalido_DebeResponderBadRequest()
    {
        await using var context = _database.CrearContexto();

        var respuesta = await CrearController(context).Crear(
            new HttpRequest { Nombre = "   " },
            CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(respuesta.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
        Assert.IsType<ProblemDetails>(badRequest.Value);
    }

    [Fact]
    [Trait("HU", "HU-06-Auditoria")]
    public async Task PostConcurrente_DeNombresEquivalentes_DebeResponderCreatedYConflict()
    {
        var sufijo = Guid.NewGuid().ToString("N");
        var barrera = new BarreraDosConsultas();

        await using var primerContexto = _database.CrearContexto();
        await using var segundoContexto = _database.CrearContexto();
        var primerController = CrearController(primerContexto, barrera);
        var segundoController = CrearController(segundoContexto, barrera);

        var respuestas = await Task.WhenAll(
            CapturarRespuestaAsync(() => primerController.Crear(
                new HttpRequest { Nombre = $"Proveedor   HTTP {sufijo}" },
                CancellationToken.None)),
            CapturarRespuestaAsync(() => segundoController.Crear(
                new HttpRequest { Nombre = $" proveedor http {sufijo} " },
                CancellationToken.None)));

        Assert.All(respuestas, respuesta => Assert.Null(respuesta.Exception));
        Assert.Contains(respuestas, respuesta => respuesta.StatusCode == StatusCodes.Status201Created);
        Assert.Contains(respuestas, respuesta => respuesta.StatusCode == StatusCodes.Status409Conflict);
    }

    private static ProveedoresController CrearController(
        LicitacionesDbContext context,
        BarreraDosConsultas barrera)
    {
        IProveedorRepository repository = new ProveedorRepository(context);
        var service = new CrearProveedorService(
            new RepositorioConConsultaSincronizada(repository, barrera));
        var controller = new ProveedoresController(service)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
        controller.HttpContext.Request.Path = "/api/v1/proveedores";
        return controller;
    }

    private static ProveedoresController CrearController(LicitacionesDbContext context)
    {
        var controller = new ProveedoresController(
            new CrearProveedorService(new ProveedorRepository(context)))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
        controller.HttpContext.Request.Path = "/api/v1/proveedores";
        return controller;
    }

    private static async Task<RespuestaCapturada> CapturarRespuestaAsync(
        Func<Task<ActionResult<ProveedorDto>>> operacion)
    {
        try
        {
            var respuesta = await operacion();
            var statusCode = Assert.IsAssignableFrom<ObjectResult>(respuesta.Result)
                .StatusCode;
            return new RespuestaCapturada(statusCode, null);
        }
        catch (Exception exception)
        {
            return new RespuestaCapturada(null, exception);
        }
    }

    private sealed record RespuestaCapturada(int? StatusCode, Exception? Exception);

    private sealed class RepositorioConConsultaSincronizada : IProveedorRepository
    {
        private readonly IProveedorRepository _inner;
        private readonly BarreraDosConsultas _barrera;

        public RepositorioConConsultaSincronizada(
            IProveedorRepository inner,
            BarreraDosConsultas barrera)
        {
            _inner = inner;
            _barrera = barrera;
        }

        public async Task<bool> ExisteNombreNormalizadoAsync(
            string nombreNormalizado,
            CancellationToken cancellationToken = default)
        {
            var existe = await _inner.ExisteNombreNormalizadoAsync(
                nombreNormalizado,
                cancellationToken);
            await _barrera.EsperarAmbasConsultasAsync(cancellationToken);
            return existe;
        }

        public Task AgregarAsync(
            Proveedor proveedor,
            CancellationToken cancellationToken = default)
        {
            return _inner.AgregarAsync(proveedor, cancellationToken);
        }
    }

    private sealed class BarreraDosConsultas
    {
        private readonly TaskCompletionSource _ambasConsultaron =
            new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int _cantidadConsultas;

        public async Task EsperarAmbasConsultasAsync(CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref _cantidadConsultas) == 2)
            {
                _ambasConsultaron.TrySetResult();
            }

            await _ambasConsultaron.Task.WaitAsync(cancellationToken);
        }
    }
}
