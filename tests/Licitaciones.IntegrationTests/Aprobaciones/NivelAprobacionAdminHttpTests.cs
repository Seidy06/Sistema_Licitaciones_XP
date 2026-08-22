using System.Globalization;
using System.Net;
using System.Net.Http.Json;

using Licitaciones.Infrastructure.Persistence;
using Licitaciones.IntegrationTests.Proveedores;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Licitaciones.IntegrationTests.Hu18;

[Collection(PostgreSqlCollection.Name)]
public sealed class NivelAprobacionAdminHttpTests
{
    private const int NivelEspecialId = 900002;

    private readonly PostgreSqlFixture _database;

    public NivelAprobacionAdminHttpTests(PostgreSqlFixture database) =>
        _database = database;

    [Fact]
    [Trait("HU", "HU-18")]
    public async Task Post_ConRangoTraslapado_DebeRechazarEnServidorSinPersistirElSegundo()
    {
        await using var context = _database.CrearContexto();
        var totalInicial = await context.NivelesAprobacion.CountAsync();

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/niveles-aprobacion",
            new
            {
                nombre = "Compras Traslapadas",
                montoMinimo = 500_000m,
                montoMaximo = 2_000_000m
            });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        await using var verificacion = _database.CrearContexto();
        Assert.Equal(
            totalInicial,
            await verificacion.NivelesAprobacion.CountAsync());
    }

    [Fact]
    [Trait("HU", "HU-18")]
    public async Task Post_ConSegundoRangoAbierto_DebeRechazarSuCreacion()
    {
        await using var context = _database.CrearContexto();
        var abiertosIniciales = await ContarRangosAbiertosAsync(context);
        Assert.Equal(1, abiertosIniciales);

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/niveles-aprobacion",
            new
            {
                nombre = "Junta Directiva Ampliada",
                montoMinimo = 50_000_000m,
                montoMaximo = (decimal?)null
            });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        await using var verificacion = _database.CrearContexto();
        Assert.Equal(
            abiertosIniciales,
            await ContarRangosAbiertosAsync(verificacion));
    }

    [Fact]
    [Trait("HU", "HU-18")]
    public async Task Get_Resolver_DebeObtenerAprobadorConsultandoLaTablaNivelesAprobacion()
    {
        try
        {
            await PrepararNivelEspecialAsync();

            await using var factory = CrearApiFactory();
            using var client = factory.CreateClient();

            var monto = 7_500_000m.ToString(CultureInfo.InvariantCulture);
            var response = await client.GetAsync(
                $"/api/v1/niveles-aprobacion/resolver?monto={monto}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var nivel = await response.Content.ReadFromJsonAsync<NivelAprobacionResponse>();

            var resuelto = Assert.IsType<NivelAprobacionResponse>(nivel);
            Assert.Equal(NivelEspecialId, resuelto.Id);
            Assert.Equal("Especial Compras", resuelto.Nombre);
        }
        finally
        {
            await RestaurarCatalogoAsync();
        }
    }

    private static async Task<int> ContarRangosAbiertosAsync(LicitacionesDbContext context) =>
        await context.NivelesAprobacion
            .CountAsync(n => n.MontoMaximo == null);

    private async Task PrepararNivelEspecialAsync()
    {
        await using var context = _database.CrearContexto();
        await context.Database.ExecuteSqlRawAsync($"""
            DELETE FROM "NivelesAprobacion" WHERE "Id" IN ({NivelEspecialId}, 2);
            """);
        await context.Database.ExecuteSqlRawAsync($"""
            INSERT INTO "NivelesAprobacion"
                ("Id", "Nombre", "MontoMinimo", "MontoMaximo", "CreatedAt", "UpdatedAt")
            VALUES
                ({NivelEspecialId}, 'Especial Compras', 7000000, 8000000, NOW(), NOW());
            """);
    }

    private async Task RestaurarCatalogoAsync()
    {
        await using var context = _database.CrearContexto();
        await context.Database.ExecuteSqlRawAsync($"""
            DELETE FROM "NivelesAprobacion" WHERE "Id" = {NivelEspecialId};
            """);
        await context.Database.ExecuteSqlRawAsync("""
            INSERT INTO "NivelesAprobacion"
                ("Id", "Nombre", "MontoMinimo", "MontoMaximo", "CreatedAt", "UpdatedAt")
            VALUES
                (2, 'Gerencial', 1000000, 10000000, '2026-01-01T00:00:00Z', '2026-01-01T00:00:00Z')
            ON CONFLICT ("Id") DO NOTHING;
            """);
    }

    private WebApplicationFactory<Program> CrearApiFactory() =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
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

    private sealed record NivelAprobacionResponse(int Id, string Nombre);
}
