using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Application.Proveedores.Editar;
using Licitaciones.Infrastructure.Persistence;
using Licitaciones.Web.Models.Proveedores;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using ApiController = Licitaciones.Api.Controllers.ProveedoresController;
using ApplicationCreateRequest = Licitaciones.Application.Proveedores.Crear.CrearProveedorRequest;
using HttpEditRequest = Licitaciones.Api.Contracts.Proveedores.EditarProveedorRequest;
using MvcController = Licitaciones.Web.Controllers.ProveedoresController;

namespace Licitaciones.IntegrationTests.Proveedores;

[Collection(PostgreSqlCollection.Name)]
public sealed class ProveedorEdicionHttpMvcTests
{
    private readonly PostgreSqlFixture _database;

    public ProveedorEdicionHttpMvcTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-07")]
    public void Api_DebeExponerPutEnApiV1ProveedoresPorId()
    {
        var action = typeof(ApiController).GetMethod(nameof(ApiController.Editar));

        Assert.NotNull(action);
        var route = Assert.IsType<HttpPutAttribute>(
            Assert.Single(action.GetCustomAttributes(typeof(HttpPutAttribute), false)));
        Assert.Equal("{id:guid}", route.Template);
    }

    [Fact]
    [Trait("HU", "HU-07")]
    public async Task Put_DatosValidos_DebeEditarYResponderOk()
    {
        var creado = await CrearProveedorAsync($"API original {Guid.NewGuid():N}");
        await using var context = _database.CrearContexto();
        var controller = CrearApiController(context);

        var response = await controller.Editar(
            creado.Id,
            new HttpEditRequest
            {
                Nombre = $"  API   editado {creado.Id:N} ",
                Version = creado.Version
            },
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(response.Result);
        var result = Assert.IsType<Licitaciones.Application.Proveedores.ProveedorDto>(ok.Value);
        Assert.Equal($"API editado {creado.Id:N}", result.Nombre);
        Assert.Equal($"API EDITADO {creado.Id:N}".ToUpperInvariant(), result.NombreNormalizado);
        Assert.True(result.Version > creado.Version);
    }

    [Fact]
    [Trait("HU", "HU-07")]
    public async Task Put_NombreDuplicado_DebeResponderConflict()
    {
        var existente = await CrearProveedorAsync($"API existente {Guid.NewGuid():N}");
        var editable = await CrearProveedorAsync($"API editable {Guid.NewGuid():N}");
        await using var context = _database.CrearContexto();

        var response = await CrearApiController(context).Editar(
            editable.Id,
            new HttpEditRequest
            {
                Nombre = $"  {existente.Nombre.ToUpperInvariant()}  ",
                Version = editable.Version
            },
            CancellationToken.None);

        var conflict = Assert.IsType<ConflictObjectResult>(response.Result);
        var details = Assert.IsType<ProblemDetails>(conflict.Value);
        Assert.Equal(StatusCodes.Status409Conflict, details.Status);
        Assert.Contains("duplicado", details.Title!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("HU", "HU-07")]
    public async Task Put_ProveedorInexistente_DebeResponderNotFound()
    {
        await using var context = _database.CrearContexto();
        var controller = CrearApiController(context);

        var response = await controller.Editar(
            Guid.NewGuid(),
            new HttpEditRequest { Nombre = "Nombre nuevo", Version = 1 },
            CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(response.Result);
    }

    [Fact]
    [Trait("HU", "HU-07")]
    public async Task Put_VersionDesactualizada_DebeResponderConflict()
    {
        var creado = await CrearProveedorAsync($"HTTP {Guid.NewGuid():N}");

        await using (var firstContext = _database.CrearContexto())
        {
            var firstService = new EditarProveedorService(new ProveedorRepository(firstContext));
            await firstService.EditarAsync(
                creado.Id,
                new Licitaciones.Application.Proveedores.Editar.EditarProveedorRequest(
                    $"Primer cambio {creado.Id:N}",
                    creado.Version));
        }

        await using var staleContext = _database.CrearContexto();
        var controller = CrearApiController(staleContext);
        var response = await controller.Editar(
            creado.Id,
            new HttpEditRequest
            {
                Nombre = $"Cambio obsoleto {creado.Id:N}",
                Version = creado.Version
            },
            CancellationToken.None);

        var conflict = Assert.IsType<ConflictObjectResult>(response.Result);
        var details = Assert.IsType<ProblemDetails>(conflict.Value);
        Assert.Equal(StatusCodes.Status409Conflict, details.Status);
        Assert.Contains("actualiz", details.Detail!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("HU", "HU-07")]
    public async Task EditGet_DebeMostrarFormularioConIdNombreYVersion()
    {
        var creado = await CrearProveedorAsync($"MVC {Guid.NewGuid():N}");
        await using var context = _database.CrearContexto();

        var response = await CrearMvcController(context).Edit(creado.Id, CancellationToken.None);

        var view = Assert.IsType<ViewResult>(response);
        var model = Assert.IsType<EditarProveedorViewModel>(view.Model);
        Assert.Equal(creado.Id, model.Id);
        Assert.Equal(creado.Nombre, model.Nombre);
        Assert.Equal(creado.Version, model.Version);
    }

    [Fact]
    [Trait("HU", "HU-07")]
    public async Task EditPost_DatosValidos_DebePersistirYRedirigirAlDetalle()
    {
        var creado = await CrearProveedorAsync($"MVC original {Guid.NewGuid():N}");
        var nombreEditado = $"MVC editado {creado.Id:N}";
        await using var context = _database.CrearContexto();
        var controller = CrearMvcController(context);
        controller.TempData = CrearTempData();

        var response = await controller.Edit(
            creado.Id,
            new EditarProveedorViewModel
            {
                Id = creado.Id,
                Nombre = nombreEditado,
                Version = creado.Version
            },
            CancellationToken.None);

        var redirect = Assert.IsType<RedirectToActionResult>(response);
        Assert.Equal(nameof(MvcController.Details), redirect.ActionName);
        Assert.Equal("El proveedor se actualizó correctamente.", controller.TempData["MensajeExito"]);

        await using var verificationContext = _database.CrearContexto();
        var guardado = await verificationContext.Proveedores.FindAsync(creado.Id);
        Assert.NotNull(guardado);
        Assert.Equal(nombreEditado, guardado.Nombre);
    }

    [Fact]
    [Trait("HU", "HU-07")]
    public async Task EditPost_VersionDesactualizada_DebeVolverAlFormularioConError()
    {
        var creado = await CrearProveedorAsync($"MVC conflicto {Guid.NewGuid():N}");

        await using (var firstContext = _database.CrearContexto())
        {
            var firstService = new EditarProveedorService(new ProveedorRepository(firstContext));
            await firstService.EditarAsync(
                creado.Id,
                new Licitaciones.Application.Proveedores.Editar.EditarProveedorRequest(
                    $"Primer cambio MVC {creado.Id:N}",
                    creado.Version));
        }

        await using var staleContext = _database.CrearContexto();
        var controller = CrearMvcController(staleContext);
        var model = new EditarProveedorViewModel
        {
            Id = creado.Id,
            Nombre = $"Cambio obsoleto MVC {creado.Id:N}",
            Version = creado.Version
        };

        var response = await controller.Edit(creado.Id, model, CancellationToken.None);

        var view = Assert.IsType<ViewResult>(response);
        Assert.Same(model, view.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.Contains(
            controller.ModelState[string.Empty]!.Errors,
            error => error.ErrorMessage.Contains("actualiz", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    [Trait("HU", "HU-07")]
    public void Mvc_DebeIncluirVistaEditConTokenDeVersion()
    {
        var solutionDirectory = FindSolutionDirectory();
        var viewPath = Path.Combine(
            solutionDirectory, "src", "Licitaciones.Web", "Views", "Proveedores", "Edit.cshtml");

        Assert.True(File.Exists(viewPath), $"No existe la vista esperada: {viewPath}");
        var markup = File.ReadAllText(viewPath);
        Assert.Contains("asp-for=\"Nombre\"", markup, StringComparison.Ordinal);
        Assert.Contains("asp-for=\"Version\"", markup, StringComparison.Ordinal);
        Assert.Contains("asp-action=\"Edit\"", markup, StringComparison.Ordinal);
    }

    private static ApiController CrearApiController(LicitacionesDbContext context)
    {
        var repository = new ProveedorRepository(context);
        var controller = new ApiController(
            new CrearProveedorService(repository),
            new ConsultarProveedorService(repository),
            new EditarProveedorService(repository))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
        controller.HttpContext.Request.Path = "/api/v1/proveedores";
        return controller;
    }

    private static MvcController CrearMvcController(LicitacionesDbContext context)
    {
        var repository = new ProveedorRepository(context);
        return new MvcController(
            new CrearProveedorService(repository),
            new ConsultarProveedorService(repository),
            new EditarProveedorService(repository));
    }

    private static Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionary CrearTempData()
    {
        return new Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionary(
            new DefaultHttpContext(),
            new TempDataProvider());
    }

    private sealed class TempDataProvider : Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) =>
            new Dictionary<string, object>();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {
        }
    }

    private async Task<Licitaciones.Application.Proveedores.ProveedorDto> CrearProveedorAsync(
        string nombre)
    {
        await using var context = _database.CrearContexto();
        return await new CrearProveedorService(new ProveedorRepository(context))
            .CrearAsync(new ApplicationCreateRequest(nombre));
    }

    private static string FindSolutionDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "Licitaciones.sln")))
        {
            directory = directory.Parent;
        }

        return Assert.IsType<DirectoryInfo>(directory).FullName;
    }
}
