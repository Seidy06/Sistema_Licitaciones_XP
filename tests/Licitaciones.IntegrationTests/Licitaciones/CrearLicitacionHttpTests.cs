using System.Net;
using System.Net.Http.Json;

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

namespace Licitaciones.IntegrationTests.Hu10;

[Collection(PostgreSqlCollection.Name)]
public sealed class CrearLicitacionHttpTests
{
    private readonly PostgreSqlFixture _database;

    public CrearLicitacionHttpTests(PostgreSqlFixture database) => _database = database;

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [Trait("HU", "HU-10")]
    public async Task Api_PostConPresupuestoNoPositivo_DebeResponderBadRequest(decimal presupuesto)
    {
        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/licitaciones",
            Solicitud($"PRESUPUESTO-{Guid.NewGuid():N}", presupuesto));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("HU", "HU-10")]
    public async Task Api_PostConCodigoEquivalente_DebeResponderConflict()
    {
        var codigo = $"hu10-http-{Guid.NewGuid():N}";
        await using (var context = _database.CrearContexto())
        {
            context.Licitaciones.Add(Licitacion.Crear(
                codigo,
                "Licitación existente",
                1000m,
                DateTimeOffset.UtcNow.AddDays(1)));
            await context.SaveChangesAsync();
        }

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            "/api/v1/licitaciones",
            Solicitud($"  {codigo.ToUpperInvariant()}  ", 1000m));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
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

    private static object Solicitud(string codigo, decimal presupuesto) => new
    {
        codigo,
        titulo = "Compra para pruebas HU-10",
        presupuesto,
        fechaCierre = DateTimeOffset.UtcNow.AddDays(1)
    };
}
