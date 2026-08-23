using Licitaciones.Api.Controllers;
using Licitaciones.Application.Common;
using Licitaciones.Application.Proveedores;
using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Infrastructure.Persistence;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using CrearProveedorRequest = Licitaciones.Application.Proveedores.Crear.CrearProveedorRequest;
using CrearProveedorService = Licitaciones.Application.Proveedores.Crear.CrearProveedorService;

namespace Licitaciones.IntegrationTests.Proveedores;

[Collection(PostgreSqlCollection.Name)]
public sealed class ProveedorConsultaHttpTests
{
    private readonly PostgreSqlFixture _database;

    public ProveedorConsultaHttpTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-09")]
    public async Task GetPorId_Existente_DebeResponder200ConDto()
    {
        await using var context = _database.CrearContexto();
        var creado = await new CrearProveedorService(new ProveedorRepository(context))
            .CrearAsync(new CrearProveedorRequest($"HTTP {Guid.NewGuid():N}"));
        var controller = CrearController(context);

        var respuesta = await controller.ObtenerPorId(creado.Id, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(respuesta.Result);
        Assert.IsType<ProveedorDto>(ok.Value);
    }

    [Fact]
    [Trait("HU", "HU-09")]
    public async Task GetPorId_Inexistente_DebeResponder404()
    {
        await using var context = _database.CrearContexto();
        var respuesta = await CrearController(context)
            .ObtenerPorId(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(respuesta.Result);
    }

    [Fact]
    [Trait("HU", "HU-09")]
    public async Task GetListado_DebeExponerPaginaDeDtos()
    {
        await using var context = _database.CrearContexto();
        var respuesta = await CrearController(context).Listar(
            pagina: 1, tamanoPagina: 10, nombre: null,
            ordenarPor: ProveedorOrden.Nombre, descendente: false,
            CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(respuesta.Result);
        Assert.IsType<PaginaResultado<ProveedorDto>>(ok.Value);
    }

    private static ProveedoresController CrearController(LicitacionesDbContext context)
    {
        var repository = new ProveedorRepository(context);
        return new ProveedoresController(
            new CrearProveedorService(repository),
            new ConsultarProveedorService(repository))
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }
}
