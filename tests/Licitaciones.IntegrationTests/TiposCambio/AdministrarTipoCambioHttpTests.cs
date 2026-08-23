using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

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
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Licitaciones.IntegrationTests.Hu19;

[Collection(PostgreSqlCollection.Name)]
public sealed class AdministrarTipoCambioHttpTests
{
    private const int TipoCambioSemillaId = 1;
    private const decimal ValorSemilla = 500m;
    private const decimal ValorNuevo = 512m;
    private const decimal MontoOfertaCrc = 8_000m;

    private static readonly DateOnly FechaSemilla = new(2026, 1, 1);
    private static readonly DateOnly FechaNuevoTipoCambio = new(2026, 8, 22);
    private static readonly DateTimeOffset FechaReferencia =
        new(2026, 8, 21, 9, 0, 0, TimeSpan.Zero);

    private readonly PostgreSqlFixture _database;

    public AdministrarTipoCambioHttpTests(PostgreSqlFixture database) =>
        _database = database;

    [Fact]
    [Trait("HU", "HU-19")]
    public async Task Post_NuevoTipoCambioActivo_DebeDesactivarElPrevio_YQuedarUnicamenteUnoActivo()
    {
        try
        {
            Assert.Equal(1, await ContarActivosAsync());

            await using var factory = CrearApiFactory();
            using var client = factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/v1/tipos-cambio", new
            {
                valor = ValorNuevo,
                fecha = FechaNuevoTipoCambio
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            await using var verificacion = _database.CrearContexto();
            var activos = await verificacion.TiposCambio
                .Where(tipo => tipo.Activo)
                .ToListAsync();
            var activo = Assert.Single(activos);
            Assert.NotEqual(TipoCambioSemillaId, activo.Id);
            Assert.Equal(ValorNuevo, activo.Valor);
            Assert.Equal(FechaNuevoTipoCambio, activo.Fecha);

            var previo = await verificacion.TiposCambio
                .SingleAsync(tipo => tipo.Id == TipoCambioSemillaId);
            Assert.False(previo.Activo);
        }
        finally
        {
            await RestaurarTipoCambioBaseAsync();
        }
    }

    [Fact]
    [Trait("HU", "HU-19")]
    public async Task Get_OfertasEnUsdTrasGuardarNuevoTipoCambio_DebeDividirPorSuValorSinModificarElMontoPersistido()
    {
        try
        {
            var escenario = await PrepararOfertaCrcAsync();

            await using var factory = CrearApiFactory();
            using var client = factory.CreateClient();

            var guardado = await client.PostAsJsonAsync("/api/v1/tipos-cambio", new
            {
                valor = ValorNuevo,
                fecha = FechaNuevoTipoCambio
            });
            Assert.Equal(HttpStatusCode.Created, guardado.StatusCode);

            var response = await client.GetAsync(
                $"/api/v1/ofertas/{escenario.OfertaId}?moneda=USD");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var oferta = await response.Content.ReadFromJsonAsync<OfertaConsultaResponse>();
            Assert.NotNull(oferta);
            Assert.Equal(MontoOfertaCrc / ValorNuevo, oferta!.Monto);

            await using var verificacion = _database.CrearContexto();
            var persistida = await verificacion.Ofertas
                .SingleAsync(registro => registro.Id == escenario.OfertaId);
            Assert.Equal(MontoOfertaCrc, persistida.Monto);
        }
        finally
        {
            await RestaurarTipoCambioBaseAsync();
        }
    }

    [Fact]
    [Trait("HU", "HU-19")]
    public async Task Get_VistaEnUsd_DebeIncluirLaFechaDelTipoDeCambioUtilizado()
    {
        var escenario = await PrepararOfertaCrcAsync();

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"/api/v1/ofertas/{escenario.OfertaId}?moneda=USD");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var oferta = await response.Content.ReadFromJsonAsync<OfertaConsultaResponse>();
        Assert.NotNull(oferta);
        Assert.Equal(ValorSemilla, oferta!.TipoCambioValor);
        Assert.Equal(FechaSemilla, oferta.TipoCambioFecha);
    }

    [Fact]
    [Trait("HU", "HU-19")]
    public async Task Get_Conversion_SinConexionExterna_DebeFuncionarConElTipoDeCambioLocal()
    {
        var escenario = await PrepararOfertaCrcAsync();

        await using var factory = CrearApiFactory(sinConexionExterna: true);
        using var client = factory.CreateClient();

        var respuestaActivo = await client.GetAsync("/api/v1/tipos-cambio/activo");
        Assert.Equal(HttpStatusCode.OK, respuestaActivo.StatusCode);
        var tipoActivo = await respuestaActivo.Content
            .ReadFromJsonAsync<TipoCambioResponse>();
        Assert.NotNull(tipoActivo);
        Assert.Equal(ValorSemilla, tipoActivo!.Valor);
        Assert.True(tipoActivo.Activo);

        var respuestaOferta = await client.GetAsync(
            $"/api/v1/ofertas/{escenario.OfertaId}?moneda=USD");
        Assert.Equal(HttpStatusCode.OK, respuestaOferta.StatusCode);
        var oferta = await respuestaOferta.Content
            .ReadFromJsonAsync<OfertaConsultaResponse>();
        Assert.NotNull(oferta);
        Assert.Equal(MontoOfertaCrc / ValorSemilla, oferta!.Monto);
    }

    private async Task<int> ContarActivosAsync()
    {
        await using var context = _database.CrearContexto();
        return await context.TiposCambio.CountAsync(tipo => tipo.Activo);
    }

    private async Task<EscenarioOferta> PrepararOfertaCrcAsync()
    {
        await using var context = _database.CrearContexto(new RelojFijo(FechaReferencia));
        var licitacion = Licitacion.Crear(
            $"HU19-{Guid.NewGuid():N}",
            "Compra para conversión CRC/USD",
            10_000m,
            FechaReferencia.AddDays(5));
        licitacion.Publicar(new RelojFijo(FechaReferencia.AddMinutes(-1)));

        var proveedor = Proveedor.Crear($"Proveedor conversor {Guid.NewGuid():N}");
        var oferta = Oferta.Crear(
            licitacion.Id,
            proveedor.Id,
            MontoOfertaCrc,
            new RelojFijo(FechaReferencia));

        context.AddRange(licitacion, proveedor, oferta);
        await context.SaveChangesAsync();

        return new EscenarioOferta(oferta.Id);
    }

    private async Task RestaurarTipoCambioBaseAsync()
    {
        await using var context = _database.CrearContexto();
        await context.Database.ExecuteSqlRawAsync("""
            DELETE FROM "TiposCambio" WHERE "Id" <> 1;
            """);
        await context.Database.ExecuteSqlRawAsync("""
            UPDATE "TiposCambio"
            SET "Activo" = TRUE, "Valor" = 500, "Fecha" = DATE '2026-01-01'
            WHERE "Id" = 1;
            """);
    }

    private WebApplicationFactory<Program> CrearApiFactory(bool sinConexionExterna = false) =>
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

                if (sinConexionExterna)
                {
                    services.AddHttpClient();
                    services.ConfigureAll<HttpClientFactoryOptions>(options =>
                        options.HttpMessageHandlerBuilderActions.Add(handler =>
                            handler.PrimaryHandler = new SinConexionExternaHandler()));
                }
            });
        });

    private sealed record EscenarioOferta(Guid OfertaId);

    private sealed record TipoCambioResponse(
        int Id,
        string MonedaOrigen,
        string MonedaDestino,
        decimal Valor,
        DateOnly Fecha,
        bool Activo);

    private sealed record OfertaConsultaResponse(
        Guid Id,
        string ProveedorNombre,
        decimal Monto,
        string Moneda,
        DateTimeOffset FechaRegistro,
        bool EsMejorOferta,
        decimal? TipoCambioValor,
        DateOnly? TipoCambioFecha);

    private sealed class RelojFijo : IClock
    {
        private readonly DateTimeOffset _valor;

        public RelojFijo(DateTimeOffset valor) => _valor = valor;

        public DateTimeOffset UtcNow() => _valor;
    }

    private sealed class SinConexionExternaHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            throw new HttpRequestException(
                "Simulación de ausencia de conexión a Internet: llamada saliente bloqueada.");
    }
}
