using System.Net;
using System.Net.Http.Json;

using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;
using Licitaciones.Domain.Proveedores;
using Licitaciones.Infrastructure.Persistence;
using Licitaciones.IntegrationTests.Proveedores;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Licitaciones.IntegrationTests.Hu14;

public sealed class CrearOfertaHttpTests : IClassFixture<PostgreSqlFixture>
{
    private static readonly DateTimeOffset Ahora =
        new(2026, 8, 19, 15, 0, 0, TimeSpan.Zero);

    private readonly PostgreSqlFixture _database;

    public CrearOfertaHttpTests(PostgreSqlFixture database) =>
        _database = database;

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [Trait("HU", "HU-14")]
    public async Task Api_PostConMontoNoPositivo_DebeResponderBadRequest(decimal monto)
    {
        var (licitacionId, proveedorId) = await PrepararLicitacionPublicada(
            $"API-PRES-{Guid.NewGuid():N}");

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/ofertas",
            Solicitud(licitacionId, proveedorId, monto));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("HU", "HU-14")]
    public async Task Api_PostConMontoMayorAlPresupuesto_DebeResponderBadRequest()
    {
        var (licitacionId, proveedorId) = await PrepararLicitacionPublicada(
            $"API-PRESUP-{Guid.NewGuid():N}");

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/ofertas",
            Solicitud(licitacionId, proveedorId, 15_000m));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("HU", "HU-14")]
    public async Task Api_PostConLicitacionNoPublicada_DebeResponderBadRequest()
    {
        var licitacionId = Guid.Empty;
        var proveedorId = Guid.Empty;

        await using (var context = _database.CrearContexto(new FixedClock(Ahora)))
        {
            var licitacion = Licitacion.Crear(
                $"API-BORRADOR-{Guid.NewGuid():N}",
                "Compra en borrador para API",
                10_000m,
                Ahora.AddDays(10));

            var proveedor = Proveedor.Crear($"API-BORR-{Guid.NewGuid():N}");

            context.Licitaciones.Add(licitacion);
            context.Proveedores.Add(proveedor);
            await context.SaveChangesAsync();

            licitacionId = licitacion.Id;
            proveedorId = proveedor.Id;
        }

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/ofertas",
            Solicitud(licitacionId, proveedorId, 500m));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("HU", "HU-14")]
    public async Task Api_PostConLicitacionPublicadaVencida_DebeResponderBadRequest()
    {
        var licitacionId = Guid.Empty;
        var proveedorId = Guid.Empty;

        await using (var context = _database.CrearContexto(new FixedClock(Ahora)))
        {
            var licitacion = Licitacion.Crear(
                $"API-VENCIDA-{Guid.NewGuid():N}",
                "Compra vencida para API",
                10_000m,
                Ahora.AddTicks(-1));

            licitacion.Publicar(new FixedClock(Ahora.AddTicks(-2)));

            var proveedor = Proveedor.Crear($"API-VENC-{Guid.NewGuid():N}");

            context.Licitaciones.Add(licitacion);
            context.Proveedores.Add(proveedor);
            await context.SaveChangesAsync();

            licitacionId = licitacion.Id;
            proveedorId = proveedor.Id;
        }

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/ofertas",
            Solicitud(licitacionId, proveedorId, 500m));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    [Trait("HU", "HU-14")]
    public async Task Api_PostDuplicada_DebeResponderConflict()
    {
        var (licitacionId, proveedorId) = await PrepararLicitacionPublicada(
            $"API-DUP-{Guid.NewGuid():N}");

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        await client.PostAsJsonAsync(
            "/api/v1/ofertas",
            Solicitud(licitacionId, proveedorId, 500m));

        var response = await client.PostAsJsonAsync(
            "/api/v1/ofertas",
            Solicitud(licitacionId, proveedorId, 600m));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    private async Task<(Guid LicitacionId, Guid ProveedorId)> PrepararLicitacionPublicada(
        string codigo)
    {
        await using var context = _database.CrearContexto(new FixedClock(Ahora));
        var licitacion = Licitacion.Crear(
            codigo,
            "Compra publicada para API",
            10_000m,
            Ahora.AddDays(10));

        licitacion.Publicar(new FixedClock(Ahora));

        var proveedor = Proveedor.Crear($"API-PUB-{Guid.NewGuid():N}");

        context.Licitaciones.Add(licitacion);
        context.Proveedores.Add(proveedor);
        await context.SaveChangesAsync();

        return (licitacion.Id, proveedor.Id);
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

    private static object Solicitud(
        Guid licitacionId, Guid proveedorId, decimal monto) => new
    {
        licitacionId,
        proveedorId,
        monto
    };

    private sealed class FixedClock : IClock
    {
        private readonly DateTimeOffset _value;
        public FixedClock(DateTimeOffset value) => _value = value;
        public DateTimeOffset UtcNow() => _value;
    }
}
