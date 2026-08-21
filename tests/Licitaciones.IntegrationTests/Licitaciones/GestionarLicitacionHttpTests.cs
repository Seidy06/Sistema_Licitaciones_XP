using System.Net;
using System.Net.Http.Json;

using Licitaciones.Application.Licitaciones;
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

namespace Licitaciones.IntegrationTests.Hu11YHu12;

[Collection(PostgreSqlCollection.Name)]
public sealed class GestionarLicitacionHttpTests
{
    private static readonly DateTimeOffset Ahora =
        new(2026, 8, 21, 12, 0, 0, TimeSpan.Zero);
    private readonly PostgreSqlFixture _database;

    public GestionarLicitacionHttpTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-11")]
    public async Task Publicar_Borrador_PersisteEstadoYTransicion()
    {
        var licitacion = await CrearBorradorAsync();
        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync(
            $"/api/v1/licitaciones/{licitacion.Id}/publicar", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await using var context = _database.CrearContexto(new FixedClock(Ahora));
        var guardada = await context.Licitaciones.Include(x => x.Transiciones)
            .SingleAsync(x => x.Id == licitacion.Id);
        Assert.Equal(EstadoLicitacion.Publicada, guardada.Estado);
        Assert.Contains(guardada.Transiciones, x => x.EstadoNuevo == EstadoLicitacion.Publicada);
    }

    [Fact]
    [Trait("HU", "HU-12")]
    public async Task Editar_Publicada_PersisteCamposPermitidos()
    {
        var licitacion = await CrearBorradorAsync(publicada: true);
        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync(
            $"/api/v1/licitaciones/{licitacion.Id}",
            new { titulo = "TÃ­tulo actualizado" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await using var context = _database.CrearContexto(new FixedClock(Ahora));
        Assert.Equal("TÃ­tulo actualizado", (await context.Licitaciones
            .SingleAsync(x => x.Id == licitacion.Id)).Titulo);
    }

    [Fact]
    [Trait("HU", "HU-12")]
    public async Task Cerrar_Publicada_PersisteEstadoYTransicion()
    {
        var licitacion = await CrearBorradorAsync(publicada: true);
        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync(
            $"/api/v1/licitaciones/{licitacion.Id}/cerrar", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        await using var context = _database.CrearContexto(new FixedClock(Ahora));
        var guardada = await context.Licitaciones.Include(x => x.Transiciones)
            .SingleAsync(x => x.Id == licitacion.Id);
        Assert.Equal(EstadoLicitacion.Cerrada, guardada.Estado);
        Assert.Contains(guardada.Transiciones, x => x.EstadoNuevo == EstadoLicitacion.Cerrada);
    }

    private async Task<Licitacion> CrearBorradorAsync(bool publicada = false)
    {
        var licitacion = Licitacion.Crear(
            $"AUD-{Guid.NewGuid():N}", "Compra auditada", 1000m, Ahora.AddDays(2));
        if (publicada)
        {
            licitacion.Publicar(new FixedClock(Ahora));
        }

        await using var context = _database.CrearContexto(new FixedClock(Ahora));
        context.Licitaciones.Add(licitacion);
        await context.SaveChangesAsync();
        return licitacion;
    }

    private WebApplicationFactory<Program> CrearApiFactory() =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureLogging(logging => logging.ClearProviders());
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<LicitacionesDbContext>>();
                services.RemoveAll<LicitacionesDbContext>();
                services.RemoveAll<IClock>();
                services.AddSingleton<IClock>(new FixedClock(Ahora));
                services.AddDbContext<LicitacionesDbContext>(options =>
                    options.UseNpgsql(_database.ConnectionString));
                services.AddDataProtection().UseEphemeralDataProtectionProvider();
            });
        });

    private sealed class FixedClock : IClock
    {
        private readonly DateTimeOffset _value;
        public FixedClock(DateTimeOffset value) => _value = value;
        public DateTimeOffset UtcNow() => _value;
    }
}
