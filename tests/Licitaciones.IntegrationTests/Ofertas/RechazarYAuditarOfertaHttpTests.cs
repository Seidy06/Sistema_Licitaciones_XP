using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;

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

namespace Licitaciones.IntegrationTests.Hu15;

[Collection(PostgreSqlCollection.Name)]
public sealed class RechazarYAuditarOfertaHttpTests
{
    private static readonly DateTimeOffset Ahora =
        new(2030, 1, 15, 12, 0, 0, TimeSpan.Zero);

    private readonly PostgreSqlFixture _database;

    public RechazarYAuditarOfertaHttpTests(PostgreSqlFixture database) =>
        _database = database;

    [Fact]
    [Trait("HU", "HU-15")]
    public async Task Api_PostOfertaDuplicada_DebeResponderConflictConMensajeEspecifico()
    {
        var (licitacion, proveedor) = await PrepararOfertaExistenteAsync(
            EstadoLicitacion.Publicada,
            Ahora.AddDays(10));

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/ofertas",
            Solicitud(licitacion.Id, proveedor.Id, 750m));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Contains(
            "oferta activa",
            await ObtenerDetalleAsync(response),
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("HU", "HU-15")]
    public async Task Api_PostOfertaSobrePresupuesto_DebeResponderUnprocessableEntityConMensajeEspecifico()
    {
        var (licitacion, proveedor) = await PrepararLicitacionAsync(
            EstadoLicitacion.Publicada,
            Ahora.AddDays(10));

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/ofertas",
            Solicitud(licitacion.Id, proveedor.Id, licitacion.Presupuesto + 1m));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Contains(
            "presupuesto",
            await ObtenerDetalleAsync(response),
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("HU", "HU-15")]
    public async Task Api_PostOfertaVencida_DebeResponderUnprocessableEntityConMensajeEspecifico()
    {
        var (licitacion, proveedor) = await PrepararLicitacionAsync(
            EstadoLicitacion.Publicada,
            DateTimeOffset.UtcNow.AddDays(-1));

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/ofertas",
            Solicitud(licitacion.Id, proveedor.Id, 500m));

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Contains(
            "vencida",
            await ObtenerDetalleAsync(response),
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("HU", "HU-15")]
    public async Task Api_PutOfertaDeLicitacionCerrada_DebeRechazarYConservarEvidencia()
    {
        var (licitacion, _, oferta) = await PrepararOfertaCerradaAsync();

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync(
            $"/api/v1/ofertas/{oferta.Id}",
            new { monto = oferta.Monto + 100m });

        var persistida = await ObtenerOfertaAsync(oferta.Id);
        Assert.Equal(oferta.Monto, persistida.Monto);
        Assert.Equal(licitacion.Id, persistida.LicitacionId);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Contains(
            "cerrada",
            await ObtenerDetalleAsync(response),
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("HU", "HU-15")]
    public async Task Api_DeleteOfertaDeLicitacionCerrada_DebeRechazarYConservarEvidencia()
    {
        var (licitacion, proveedor, oferta) = await PrepararOfertaCerradaAsync();

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync($"/api/v1/ofertas/{oferta.Id}");

        var persistida = await ObtenerOfertaAsync(oferta.Id);
        Assert.Equal(licitacion.Id, persistida.LicitacionId);
        Assert.Equal(proveedor.Id, persistida.ProveedorId);
        Assert.Equal(oferta.Monto, persistida.Monto);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Contains(
            "cerrada",
            await ObtenerDetalleAsync(response),
            StringComparison.OrdinalIgnoreCase);
    }

    private async Task<(Licitacion Licitacion, Proveedor Proveedor)>
        PrepararLicitacionAsync(
            EstadoLicitacion estado,
            DateTimeOffset fechaCierre)
    {
        var licitacion = Licitacion.Crear(
            $"HU15-{Guid.NewGuid():N}",
            "Compra para rechazo de oferta",
            10_000m,
            fechaCierre);
        EstablecerEstado(licitacion, estado);

        var proveedor = Proveedor.Crear($"Proveedor HU15 {Guid.NewGuid():N}");

        await using var context = _database.CrearContexto(new FixedClock(Ahora));
        context.AddRange(licitacion, proveedor);
        await context.SaveChangesAsync();

        return (licitacion, proveedor);
    }

    private async Task<(Licitacion Licitacion, Proveedor Proveedor)>
        PrepararOfertaExistenteAsync(
            EstadoLicitacion estado,
            DateTimeOffset fechaCierre)
    {
        var (licitacion, proveedor) = await PrepararLicitacionAsync(
            estado, fechaCierre);

        await using var context = _database.CrearContexto(new FixedClock(Ahora));
        context.Ofertas.Add(Oferta.Crear(
            licitacion.Id, proveedor.Id, 500m, new FixedClock(Ahora)));
        await context.SaveChangesAsync();

        return (licitacion, proveedor);
    }

    private async Task<(Licitacion Licitacion, Proveedor Proveedor, Oferta Oferta)>
        PrepararOfertaCerradaAsync()
    {
        var (licitacion, proveedor) = await PrepararLicitacionAsync(
            EstadoLicitacion.Cerrada,
            Ahora.AddDays(10));
        var oferta = Oferta.Crear(
            licitacion.Id, proveedor.Id, 500m, new FixedClock(Ahora));

        await using var context = _database.CrearContexto(new FixedClock(Ahora));
        context.Ofertas.Add(oferta);
        await context.SaveChangesAsync();

        return (licitacion, proveedor, oferta);
    }

    private async Task<Oferta> ObtenerOfertaAsync(Guid id)
    {
        await using var context = _database.CrearContexto(new FixedClock(Ahora));
        return await context.Ofertas.AsNoTracking().SingleAsync(x => x.Id == id);
    }

    private WebApplicationFactory<Licitaciones.Api.Controllers.OfertasController>
        CrearApiFactory() =>
        new WebApplicationFactory<Licitaciones.Api.Controllers.OfertasController>()
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

    private static object Solicitud(
        Guid licitacionId,
        Guid proveedorId,
        decimal monto) => new
        {
            licitacionId,
            proveedorId,
            monto
        };

    private static async Task<string> ObtenerDetalleAsync(HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.GetProperty("detail").GetString() ?? string.Empty;
    }

    private static void EstablecerEstado(
        Licitacion licitacion,
        EstadoLicitacion estado) =>
        typeof(Licitacion)
            .GetProperty(
                nameof(Licitacion.Estado),
                BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(licitacion, estado);

    private sealed class FixedClock : IClock
    {
        private readonly DateTimeOffset _value;

        public FixedClock(DateTimeOffset value) => _value = value;

        public DateTimeOffset UtcNow() => _value;
    }
}
