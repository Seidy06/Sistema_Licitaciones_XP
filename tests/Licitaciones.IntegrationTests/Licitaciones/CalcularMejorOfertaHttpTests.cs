using System.Net;
using System.Text.Json;

using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;
using Licitaciones.Domain.Proveedores;
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

namespace Licitaciones.IntegrationTests.Hu16;

public sealed class CalcularMejorOfertaHttpTests : IClassFixture<PostgreSqlFixture>
{
    private static readonly DateTimeOffset Ahora =
        new(2026, 8, 20, 12, 0, 0, TimeSpan.Zero);

    private readonly PostgreSqlFixture _database;

    public CalcularMejorOfertaHttpTests(PostgreSqlFixture database) =>
        _database = database;

    [Fact]
    [Trait("HU", "HU-16")]
    public async Task Api_VariasOfertasConEmpate_DebeSeleccionarMenorMontoRegistradoPrimero()
    {
        var licitacion = CrearLicitacionPublicada(10_000m);
        var proveedorMayor = Proveedor.Crear($"Proveedor mayor {Guid.NewGuid():N}");
        var proveedorPrimero = Proveedor.Crear($"Proveedor primero {Guid.NewGuid():N}");
        var proveedorDespues = Proveedor.Crear($"Proveedor después {Guid.NewGuid():N}");
        var ofertaMayor = Oferta.Crear(
            licitacion.Id, proveedorMayor.Id, 9_500m, new FixedClock(Ahora));
        var ofertaPrimero = Oferta.Crear(
            licitacion.Id, proveedorPrimero.Id, 9_000m, new FixedClock(Ahora.AddMinutes(1)));
        var ofertaDespues = Oferta.Crear(
            licitacion.Id, proveedorDespues.Id, 9_000m, new FixedClock(Ahora.AddMinutes(2)));

        await GuardarAsync(
            licitacion,
            [proveedorMayor, proveedorPrimero, proveedorDespues],
            [ofertaMayor, ofertaPrimero, ofertaDespues]);

        var detalle = await ObtenerDetalleAsync(licitacion.Id);
        var mejorOferta = detalle.GetProperty("mejorOferta");

        Assert.Equal(9_000m, mejorOferta.GetProperty("monto").GetDecimal());
        Assert.True(
            mejorOferta.TryGetProperty("id", out var ofertaId),
            "La mejor oferta debe identificar cuál oferta ganó el desempate.");
        Assert.Equal(ofertaPrimero.Id, ofertaId.GetGuid());
    }

    [Fact]
    [Trait("HU", "HU-16")]
    public async Task Api_SinOfertasValidas_DebeMostrarMensajeEspecifico()
    {
        var licitacion = CrearLicitacionPublicada(10_000m);
        await GuardarAsync(licitacion, [], []);

        var contenido = await ObtenerContenidoDetalleAsync(licitacion.Id);

        Assert.Contains("Sin ofertas válidas", contenido, StringComparison.Ordinal);
    }

    [Fact]
    [Trait("HU", "HU-16")]
    public async Task Api_AhorroExactamenteDiezPorCiento_DebeClasificarOfertaConveniente()
    {
        var contenido = await PrepararYConsultarOfertaAsync(
            presupuesto: 10_000m,
            montoOferta: 9_000m);

        Assert.Contains("Oferta conveniente", contenido, StringComparison.Ordinal);
    }

    [Fact]
    [Trait("HU", "HU-16")]
    public async Task Api_AhorroMayorACeroYMenorADiezPorCiento_DebeClasificarOfertaAceptable()
    {
        var contenido = await PrepararYConsultarOfertaAsync(
            presupuesto: 10_000m,
            montoOferta: 9_500m);

        Assert.Contains("Oferta aceptable", contenido, StringComparison.Ordinal);
    }

    [Fact]
    [Trait("HU", "HU-16")]
    public async Task Api_OfertaIgualAlPresupuesto_DebeClasificarValidaSinAhorro()
    {
        var contenido = await PrepararYConsultarOfertaAsync(
            presupuesto: 10_000m,
            montoOferta: 10_000m);

        Assert.Contains("Oferta válida sin ahorro", contenido, StringComparison.Ordinal);
    }

    private async Task<string> PrepararYConsultarOfertaAsync(
        decimal presupuesto,
        decimal montoOferta)
    {
        var licitacion = CrearLicitacionPublicada(presupuesto);
        var proveedor = Proveedor.Crear($"Proveedor HU16 {Guid.NewGuid():N}");
        var oferta = Oferta.Crear(
            licitacion.Id,
            proveedor.Id,
            montoOferta,
            new FixedClock(Ahora));

        await GuardarAsync(licitacion, [proveedor], [oferta]);
        return await ObtenerContenidoDetalleAsync(licitacion.Id);
    }

    private Licitacion CrearLicitacionPublicada(decimal presupuesto)
    {
        var licitacion = Licitacion.Crear(
            $"HU16-{Guid.NewGuid():N}",
            "Compra para calcular mejor oferta",
            presupuesto,
            Ahora.AddDays(5));
        licitacion.Publicar(new FixedClock(Ahora));
        return licitacion;
    }

    private async Task GuardarAsync(
        Licitacion licitacion,
        IReadOnlyCollection<Proveedor> proveedores,
        IReadOnlyCollection<Oferta> ofertas)
    {
        await using var context = _database.CrearContexto();
        context.Licitaciones.Add(licitacion);
        context.Proveedores.AddRange(proveedores);
        context.Ofertas.AddRange(ofertas);
        await context.SaveChangesAsync();
    }

    private async Task<JsonElement> ObtenerDetalleAsync(Guid licitacionId)
    {
        var contenido = await ObtenerContenidoDetalleAsync(licitacionId);
        using var document = JsonDocument.Parse(contenido);
        return document.RootElement.Clone();
    }

    private async Task<string> ObtenerContenidoDetalleAsync(Guid licitacionId)
    {
        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/v1/licitaciones/{licitacionId}");
        var contenido = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return contenido;
    }

    private WebApplicationFactory<Licitaciones.Api.Controllers.LicitacionesController>
        CrearApiFactory() =>
        new WebApplicationFactory<Licitaciones.Api.Controllers.LicitacionesController>()
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

    private sealed class FixedClock : Licitaciones.Domain.Common.IClock
    {
        private readonly DateTimeOffset _value;

        public FixedClock(DateTimeOffset value) => _value = value;

        public DateTimeOffset UtcNow() => _value;
    }
}
