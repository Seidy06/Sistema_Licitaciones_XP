using System.Net;
using System.Net.Http.Json;

using Licitaciones.Domain.Licitaciones;
using Licitaciones.Infrastructure.Persistence;
using Licitaciones.IntegrationTests.Common;
using Licitaciones.IntegrationTests.Proveedores;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using static Licitaciones.IntegrationTests.Common.LicitacionTestHelper;

namespace Licitaciones.IntegrationTests.Hu13;

[Collection(PostgreSqlCollection.Name)]
public sealed class ConsultarLicitacionHttpTests
{
    private static readonly DateTimeOffset Ahora =
        new(2026, 8, 19, 12, 0, 0, TimeSpan.Zero);

    private readonly PostgreSqlFixture _database;

    public ConsultarLicitacionHttpTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task Api_GetListar_DebeRetornarOk()
    {
        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/licitaciones");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task Api_GetListar_ConCodigoOrdenYPaginacion_DebeAplicarConsulta()
    {
        var prefijo = $"FILTRO-{Guid.NewGuid():N}";
        await using (var context = _database.CrearContexto())
        {
            context.Licitaciones.AddRange(
                Licitacion.Crear($"{prefijo}-A", "Primera", 2000m, Ahora.AddDays(2)),
                Licitacion.Crear($"{prefijo}-B", "Segunda", 1000m, Ahora.AddDays(1)));
            await context.SaveChangesAsync();
        }

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();
        var response = await client.GetAsync(
            $"/api/v1/licitaciones?codigo={prefijo}&ordenarPor=presupuesto" +
            "&descendente=true&pagina=1&tamanoPagina=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var pagina = await response.Content.ReadFromJsonAsync<PaginaResponse>();
        Assert.NotNull(pagina);
        Assert.Equal(2, pagina.Total);
        Assert.Equal("Primera", Assert.Single(pagina.Items).Titulo);
    }

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task Api_GetDetalle_DebeRetornarOk()
    {
        var licitacion = PublicarLicitacion($"HTTP-{Guid.NewGuid():N}", Ahora.AddDays(5));
        await using (var context = _database.CrearContexto())
        {
            context.Licitaciones.Add(licitacion);
            await context.SaveChangesAsync();
        }

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/v1/licitaciones/{licitacion.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task Api_GetDetalle_Inexistente_DebeRetornarNotFound()
    {
        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/v1/licitaciones/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private WebApplicationFactory<Licitaciones.Api.Controllers.ProveedoresController>
        CrearApiFactory()
    {
        return new WebApplicationFactory<Licitaciones.Api.Controllers.ProveedoresController>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureLogging(logging => logging.ClearProviders());
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<LicitacionesDbContext>>();
                    services.RemoveAll<LicitacionesDbContext>();
                    services.AddDbContext<LicitacionesDbContext>(options =>
                        options.UseNpgsql(_database.ConnectionString));
                    services.AddDataProtection().UseEphemeralDataProtectionProvider();
                });
            });
    }

    private sealed record PaginaResponse(
        IReadOnlyList<ItemResponse> Items,
        int Total,
        int Pagina,
        int TamanoPagina);

    private sealed record ItemResponse(Guid Id, string Titulo, decimal Presupuesto);
}
