using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Application.Proveedores.Editar;
using Licitaciones.Application.Proveedores.Eliminar;
using Licitaciones.Infrastructure.Persistence;
using Licitaciones.Infrastructure.Time;
using Licitaciones.Web.Models.Proveedores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ApiController = Licitaciones.Api.Controllers.ProveedoresController;
using MvcController = Licitaciones.Web.Controllers.ProveedoresController;

namespace Licitaciones.IntegrationTests.Proveedores;

public sealed class ProveedorBajaHttpMvcTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _database;

    public ProveedorBajaHttpMvcTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-08")]
    public void Api_DebeExponerDeleteEnApiV1ProveedoresPorId()
    {
        var action = typeof(ApiController).GetMethod(nameof(ApiController.Eliminar));

        Assert.NotNull(action);
        var route = Assert.IsType<HttpDeleteAttribute>(
            Assert.Single(action.GetCustomAttributes(typeof(HttpDeleteAttribute), false)));
        Assert.Equal("{id:guid}", route.Template);
    }

    [Fact]
    [Trait("HU", "HU-08")]
    public async Task Delete_ProveedorActivo_DebeResponderNoContentYOcultarloDelListadoActivo()
    {
        var creado = await CrearProveedorAsync($"API baja {Guid.NewGuid():N}");
        await using var context = _database.CrearContexto();

        var response = await CrearApiController(context)
            .Eliminar(creado.Id, CancellationToken.None);

        Assert.IsType<NoContentResult>(response);

        var listado = await CrearApiController(context).Listar(
            pagina: 1,
            tamanoPagina: 20,
            nombre: creado.Nombre,
            ordenarPor: ProveedorOrden.Nombre,
            descendente: false,
            CancellationToken.None);
        var ok = Assert.IsType<OkObjectResult>(listado.Result);
        var pagina = Assert.IsType<PaginaResultado<Licitaciones.Application.Proveedores.ProveedorDto>>(
            ok.Value);
        Assert.DoesNotContain(pagina.Items, proveedor => proveedor.Id == creado.Id);

        await using var verificationContext = _database.CrearContexto();
        Assert.False(await verificationContext.Proveedores.AnyAsync(
            proveedor => proveedor.Id == creado.Id));
    }

    [Fact]
    [Trait("HU", "HU-08")]
    public async Task Delete_ProveedorInexistente_DebeResponderNotFound()
    {
        await using var context = _database.CrearContexto();

        var response = await CrearApiController(context)
            .Eliminar(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(response);
    }

    [Fact]
    [Trait("HU", "HU-08")]
    public async Task DeleteGet_DebeMostrarConfirmacionConDatosDelProveedor()
    {
        var creado = await CrearProveedorAsync($"MVC baja {Guid.NewGuid():N}");
        await using var context = _database.CrearContexto();

        var response = await CrearMvcController(context)
            .Delete(creado.Id, CancellationToken.None);

        var view = Assert.IsType<ViewResult>(response);
        var model = Assert.IsType<EliminarProveedorViewModel>(view.Model);
        Assert.Equal(creado.Id, model.Id);
        Assert.Equal(creado.Nombre, model.Nombre);
    }

    [Fact]
    [Trait("HU", "HU-08")]
    public async Task DeleteConfirmed_DebeDarDeBajaYRedirigirAlListado()
    {
        var creado = await CrearProveedorAsync($"MVC confirmada {Guid.NewGuid():N}");
        await using var context = _database.CrearContexto();

        var response = await CrearMvcController(context)
            .DeleteConfirmed(creado.Id, CancellationToken.None);

        var redirect = Assert.IsType<RedirectToActionResult>(response);
        Assert.Equal(nameof(MvcController.Index), redirect.ActionName);

        await using var verificationContext = _database.CrearContexto();
        Assert.False(await verificationContext.Proveedores.AnyAsync(
            proveedor => proveedor.Id == creado.Id));
    }

    [Fact]
    [Trait("HU", "HU-08")]
    public void Mvc_DebeIncluirVistaDeConfirmacionPrevia()
    {
        var viewPath = Path.Combine(
            FindSolutionDirectory(), "src", "Licitaciones.Web", "Views",
            "Proveedores", "Delete.cshtml");

        Assert.True(File.Exists(viewPath), $"No existe la vista esperada: {viewPath}");
        var markup = File.ReadAllText(viewPath);
        Assert.Contains("asp-action=\"DeleteConfirmed\"", markup, StringComparison.Ordinal);
        Assert.Contains("asp-for=\"Id\"", markup, StringComparison.Ordinal);
        Assert.Contains("confirm", markup, StringComparison.OrdinalIgnoreCase);
    }

    private static ApiController CrearApiController(LicitacionesDbContext context)
    {
        var repository = new ProveedorRepository(context);
        return new ApiController(
            new CrearProveedorService(repository),
            new ConsultarProveedorService(repository),
            new EditarProveedorService(repository),
            new DarBajaProveedorService(repository, new SystemClock()))
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private static MvcController CrearMvcController(LicitacionesDbContext context)
    {
        var repository = new ProveedorRepository(context);
        return new MvcController(
            new CrearProveedorService(repository),
            new ConsultarProveedorService(repository),
            new EditarProveedorService(repository),
            new DarBajaProveedorService(repository, new SystemClock()));
    }

    private async Task<Licitaciones.Application.Proveedores.ProveedorDto> CrearProveedorAsync(
        string nombre)
    {
        await using var context = _database.CrearContexto();
        return await new CrearProveedorService(new ProveedorRepository(context))
            .CrearAsync(new CrearProveedorRequest(nombre));
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
}
