using System.ComponentModel.DataAnnotations;
using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Infrastructure.Persistence;
using Licitaciones.Web.Models.Proveedores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using MvcController = Licitaciones.Web.Controllers.ProveedoresController;

namespace Licitaciones.IntegrationTests.Proveedores;

public sealed class ProveedorCreacionMvcTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _database;

    public ProveedorCreacionMvcTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    public void CreateGet_DebeMostrarFormularioDeCreacion()
    {
        using var context = _database.CrearContexto();

        var response = CrearController(context).Create();

        var view = Assert.IsType<ViewResult>(response);
        Assert.IsType<CrearProveedorViewModel>(view.Model);
    }

    [Fact]
    public async Task CreatePost_DatosValidos_DebePersistirYMostrarMensajeDeExito()
    {
        var nombre = $"MVC creación {Guid.NewGuid():N}";
        await using var context = _database.CrearContexto();
        var controller = CrearController(context);

        var response = await controller.Create(
            new CrearProveedorViewModel { Nombre = nombre },
            CancellationToken.None);

        var redirect = Assert.IsType<RedirectToActionResult>(response);
        Assert.Equal(nameof(MvcController.Create), redirect.ActionName);
        Assert.Equal("El proveedor se registró correctamente.", controller.TempData["MensajeExito"]);

        await using var verificationContext = _database.CrearContexto();
        Assert.Contains(
            verificationContext.Proveedores,
            proveedor => proveedor.Nombre == nombre);
    }

    [Fact]
    public async Task CreatePost_NombreDuplicado_DebeMostrarMensajeDeValidacion()
    {
        var nombre = $"MVC duplicado {Guid.NewGuid():N}";
        await using var context = _database.CrearContexto();
        var controller = CrearController(context);
        await controller.Create(
            new CrearProveedorViewModel { Nombre = nombre },
            CancellationToken.None);

        var model = new CrearProveedorViewModel { Nombre = nombre.ToUpperInvariant() };
        var response = await controller.Create(model, CancellationToken.None);

        var view = Assert.IsType<ViewResult>(response);
        Assert.Same(model, view.Model);
        var error = Assert.Single(controller.ModelState[nameof(model.Nombre)]!.Errors);
        Assert.Equal("Ya existe un proveedor con ese nombre.", error.ErrorMessage);
    }

    [Fact]
    public async Task CreatePost_NombreRequerido_DebeVolverConMensajeDeValidacion()
    {
        await using var context = _database.CrearContexto();
        var controller = CrearController(context);
        var model = new CrearProveedorViewModel { Nombre = string.Empty };
        AgregarErroresDeValidacion(controller, model);

        var response = await controller.Create(model, CancellationToken.None);

        var view = Assert.IsType<ViewResult>(response);
        Assert.Same(model, view.Model);
        var error = Assert.Single(controller.ModelState[nameof(model.Nombre)]!.Errors);
        Assert.Equal("El nombre del proveedor es obligatorio.", error.ErrorMessage);
    }

    [Fact]
    public void CreateView_DebeRenderizarMensajesDeValidacionDelNombre()
    {
        var viewPath = Path.Combine(
            FindSolutionDirectory(), "src", "Licitaciones.Web", "Views",
            "Proveedores", "Create.cshtml");

        var markup = File.ReadAllText(viewPath);

        Assert.Contains("asp-validation-summary=\"ModelOnly\"", markup, StringComparison.Ordinal);
        Assert.Contains("asp-validation-for=\"Nombre\"", markup, StringComparison.Ordinal);
    }

    private static MvcController CrearController(LicitacionesDbContext context)
    {
        var repository = new ProveedorRepository(context);
        var controller = new MvcController(
            new CrearProveedorService(repository),
            new ConsultarProveedorService(repository))
        {
            TempData = new TempDataDictionary(
                new DefaultHttpContext(),
                new TempDataProvider())
        };

        return controller;
    }

    private static void AgregarErroresDeValidacion(
        Controller controller,
        CrearProveedorViewModel model)
    {
        var resultados = new List<ValidationResult>();
        Validator.TryValidateObject(
            model,
            new ValidationContext(model),
            resultados,
            validateAllProperties: true);

        foreach (var resultado in resultados)
        {
            foreach (var miembro in resultado.MemberNames)
            {
                controller.ModelState.AddModelError(miembro, resultado.ErrorMessage!);
            }
        }
    }

    private static string FindSolutionDirectory()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null
            && !File.Exists(Path.Combine(directory.FullName, "Licitaciones.sln")))
        {
            directory = directory.Parent;
        }

        return Assert.IsType<DirectoryInfo>(directory).FullName;
    }

    private sealed class TempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) =>
            new Dictionary<string, object>();

        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {
        }
    }
}
