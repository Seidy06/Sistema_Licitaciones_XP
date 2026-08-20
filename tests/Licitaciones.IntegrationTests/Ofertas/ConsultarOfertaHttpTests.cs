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

namespace Licitaciones.IntegrationTests.Hu17;

public sealed class ConsultarOfertaHttpTests : IClassFixture<PostgreSqlFixture>
{
    private static readonly DateTimeOffset FechaPrimeraOferta =
        new(2026, 8, 20, 14, 30, 0, TimeSpan.Zero);

    private readonly PostgreSqlFixture _database;

    public ConsultarOfertaHttpTests(PostgreSqlFixture database) =>
        _database = database;

    [Fact]
    [Trait("HU", "HU-17")]
    public async Task GetPorLicitacion_DebeMostrarProveedorMontoCrcFechaYMejorOferta()
    {
        var escenario = await PrepararLicitacionConOfertasAsync();

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/v1/ofertas?licitacionId={escenario.LicitacionId}&moneda=CRC");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var ofertas = await response.Content.ReadFromJsonAsync<List<OfertaConsultaResponse>>();
        Assert.NotNull(ofertas);
        Assert.Collection(
            ofertas,
            oferta => AssertOferta(
                oferta,
                escenario.OfertaMejorId,
                escenario.ProveedorMejor,
                8_000m,
                "CRC",
                FechaPrimeraOferta,
                true),
            oferta => AssertOferta(
                oferta,
                escenario.OfertaMayorId,
                escenario.ProveedorMayor,
                9_000m,
                "CRC",
                FechaPrimeraOferta.AddMinutes(5),
                false));
    }

    [Fact]
    [Trait("HU", "HU-17")]
    public async Task GetPorId_EnUsd_DebeConvertirMontoSinPerderDatosDeLaOferta()
    {
        var escenario = await PrepararLicitacionConOfertasAsync();

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/v1/ofertas/{escenario.OfertaMejorId}?moneda=USD");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var oferta = await response.Content.ReadFromJsonAsync<OfertaConsultaResponse>();
        AssertOferta(
            Assert.IsType<OfertaConsultaResponse>(oferta),
            escenario.OfertaMejorId,
            escenario.ProveedorMejor,
            16m,
            "USD",
            FechaPrimeraOferta,
            true);
    }

    private async Task<Escenario> PrepararLicitacionConOfertasAsync()
    {
        await using var context = _database.CrearContexto(new FixedClock(FechaPrimeraOferta));
        var licitacion = Licitacion.Crear(
            $"HU17-{Guid.NewGuid():N}",
            "Compra para consultar ofertas",
            10_000m,
            FechaPrimeraOferta.AddDays(5));
        licitacion.Publicar(new FixedClock(FechaPrimeraOferta.AddMinutes(-1)));

        var proveedorMejor = Proveedor.Crear($"Proveedor mejor {Guid.NewGuid():N}");
        var proveedorMayor = Proveedor.Crear($"Proveedor mayor {Guid.NewGuid():N}");
        var ofertaMejor = Oferta.Crear(
            licitacion.Id,
            proveedorMejor.Id,
            8_000m,
            new FixedClock(FechaPrimeraOferta));
        var ofertaMayor = Oferta.Crear(
            licitacion.Id,
            proveedorMayor.Id,
            9_000m,
            new FixedClock(FechaPrimeraOferta.AddMinutes(5)));

        context.AddRange(licitacion, proveedorMejor, proveedorMayor, ofertaMejor, ofertaMayor);
        await context.SaveChangesAsync();

        return new Escenario(
            licitacion.Id,
            ofertaMejor.Id,
            ofertaMayor.Id,
            proveedorMejor.Nombre,
            proveedorMayor.Nombre);
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

    private static void AssertOferta(
        OfertaConsultaResponse oferta,
        Guid id,
        string proveedorNombre,
        decimal monto,
        string moneda,
        DateTimeOffset fechaRegistro,
        bool esMejorOferta)
    {
        Assert.Equal(id, oferta.Id);
        Assert.Equal(proveedorNombre, oferta.ProveedorNombre);
        Assert.Equal(monto, oferta.Monto);
        Assert.Equal(moneda, oferta.Moneda);
        Assert.Equal(fechaRegistro, oferta.FechaRegistro);
        Assert.Equal(esMejorOferta, oferta.EsMejorOferta);
    }

    private sealed record Escenario(
        Guid LicitacionId,
        Guid OfertaMejorId,
        Guid OfertaMayorId,
        string ProveedorMejor,
        string ProveedorMayor);

    private sealed record OfertaConsultaResponse(
        Guid Id,
        string ProveedorNombre,
        decimal Monto,
        string Moneda,
        DateTimeOffset FechaRegistro,
        bool EsMejorOferta);

    private sealed class FixedClock : IClock
    {
        private readonly DateTimeOffset _value;

        public FixedClock(DateTimeOffset value) => _value = value;

        public DateTimeOffset UtcNow() => _value;
    }
}
