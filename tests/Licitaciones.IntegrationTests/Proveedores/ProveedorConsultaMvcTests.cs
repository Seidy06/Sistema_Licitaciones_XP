using Licitaciones.Application.Common;
using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Infrastructure.Persistence;
using Licitaciones.Web.Models.Proveedores;

using Microsoft.AspNetCore.Mvc;

using CrearProveedorService = Licitaciones.Application.Proveedores.Crear.CrearProveedorService;
using MvcController = Licitaciones.Web.Controllers.ProveedoresController;

namespace Licitaciones.IntegrationTests.Proveedores;

[Collection(PostgreSqlCollection.Name)]
public sealed class ProveedorConsultaMvcTests
{
    private readonly PostgreSqlFixture _database;

    public ProveedorConsultaMvcTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-09")]
    public async Task Index_DebeRenderizarListadoPaginado()
    {
        var creado = await CrearProveedorAsync($"MVC listado {Guid.NewGuid():N}");
        await using var context = _database.CrearContexto();
        var controller = CrearController(context);

        var resultado = await controller.Index(
            1, 20, creado.Nombre, ProveedorOrden.Nombre, false, CancellationToken.None);

        var vista = Assert.IsType<ViewResult>(resultado);
        var modelo = Assert.IsType<PaginaResultado<ProveedorResumenViewModel>>(vista.Model);
        Assert.Contains(modelo.Items, proveedor => proveedor.Id == creado.Id);
    }

    [Fact]
    [Trait("HU", "HU-09")]
    public async Task Details_Existente_DebeMostrarDatosDelProveedor()
    {
        var creado = await CrearProveedorAsync($"MVC detalle {Guid.NewGuid():N}");
        await using var context = _database.CrearContexto();

        var resultado = await CrearController(context)
            .Details(creado.Id, CancellationToken.None);

        var vista = Assert.IsType<ViewResult>(resultado);
        var modelo = Assert.IsType<ProveedorDetalleViewModel>(vista.Model);
        Assert.Equal(creado.Id, modelo.Id);
        Assert.Equal(creado.Nombre, modelo.Nombre);
    }

    [Fact]
    [Trait("HU", "HU-09")]
    public async Task Details_Inexistente_DebeRetornarNotFound()
    {
        await using var context = _database.CrearContexto();

        var resultado = await CrearController(context).Details(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(resultado);
    }

    private static MvcController CrearController(LicitacionesDbContext context)
    {
        var repository = new ProveedorRepository(context);
        return new MvcController(
            new CrearProveedorService(repository),
            new ConsultarProveedorService(repository));
    }

    private async Task<Licitaciones.Application.Proveedores.ProveedorDto> CrearProveedorAsync(
        string nombre)
    {
        await using var context = _database.CrearContexto();
        return await new CrearProveedorService(new ProveedorRepository(context))
            .CrearAsync(new Licitaciones.Application.Proveedores.Crear.CrearProveedorRequest(nombre));
    }
}
