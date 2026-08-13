using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Infrastructure.Persistence;
using Licitaciones.Web.Models.Proveedores;
using Microsoft.AspNetCore.Mvc;

using CrearProveedorService = Licitaciones.Application.Proveedores.Crear.CrearProveedorService;
using MvcController = Licitaciones.Web.Controllers.ProveedoresController;

namespace Licitaciones.IntegrationTests.Proveedores;

public sealed class ProveedorConsultaMvcTests : IClassFixture<PostgreSqlFixture>
{
    private readonly PostgreSqlFixture _database;

    public ProveedorConsultaMvcTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-09")]
    public async Task Index_DebeRenderizarListadoPaginado()
    {
        await using var context = _database.CrearContexto();
        var controller = CrearController(context);

        var resultado = await controller.Index(1, 20, null, ProveedorOrden.Nombre, false, CancellationToken.None);

        var vista = Assert.IsType<ViewResult>(resultado);
        Assert.IsType<PaginaResultado<ProveedorResumenViewModel>>(vista.Model);
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
}
