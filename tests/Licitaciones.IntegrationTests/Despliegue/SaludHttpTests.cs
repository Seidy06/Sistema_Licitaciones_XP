using System.Net;

using Licitaciones.Infrastructure.Persistence;

using Licitaciones.IntegrationTests.Proveedores;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Licitaciones.IntegrationTests.Despliegue;

[Collection(PostgreSqlCollection.Name)]
public sealed class SaludHttpTests
{
    private readonly PostgreSqlFixture _database;

    public SaludHttpTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-31")]
    public async Task HealthEndpoint_DebeResponderHealthy()
    {
        await using var factory = CrearApiFactory();
        using var cliente = factory.CreateClient();

        var respuesta = await cliente.GetAsync("/health");

        Assert.True(
            respuesta.IsSuccessStatusCode,
            $"El endpoint '/health' debe responder 200 Healthy; "
            + $"se obtuvo {(int)respuesta.StatusCode} {respuesta.StatusCode}.");

        var cuerpo = await respuesta.Content.ReadAsStringAsync();

        Assert.Contains("healthy", cuerpo, StringComparison.OrdinalIgnoreCase);
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
}
