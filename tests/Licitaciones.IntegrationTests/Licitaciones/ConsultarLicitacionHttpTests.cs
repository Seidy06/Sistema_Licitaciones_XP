using System.Net;
using System.Net.Http.Json;

using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Infrastructure.Persistence;
using Licitaciones.IntegrationTests.Proveedores;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Licitaciones.IntegrationTests.Hu13;

public sealed class ConsultarLicitacionHttpTests : IClassFixture<PostgreSqlFixture>
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

    private static Licitacion PublicarLicitacion(
        string codigo, DateTimeOffset fechaCierre)
    {
        var licitacion = Licitacion.Crear(
            codigo,
            "Compra para pruebas HU-13",
            10_000m,
            fechaCierre);

        licitacion.Publicar(new FixedClock(fechaCierre.AddDays(-5)));
        return licitacion;
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

    private sealed class FixedClock : IClock
    {
        private readonly DateTimeOffset _value;
        public FixedClock(DateTimeOffset value) => _value = value;
        public DateTimeOffset UtcNow() => _value;
    }
}
