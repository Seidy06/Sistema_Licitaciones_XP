using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Domain.Licitaciones;
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

namespace Licitaciones.IntegrationTests.Hu26;

[Collection(PostgreSqlCollection.Name)]
public sealed class ContratoApiRestHttpTests
{
    private const string ClaveCodigoError = "codigoError";
    private const string ClaveCorrelacionId = "correlacionId";

    private readonly PostgreSqlFixture _database;

    public ContratoApiRestHttpTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-26")]
    public async Task Error_BadRequest_DebeUsarProblemDetailsConTituloEstadoDetalleCodigoYCorrelacion()
    {
        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var respuesta = await client.PostAsJsonAsync(
            "/api/v1/proveedores",
            new { nombre = "   " });

        await VerificarContratoProblemDetailsAsync(respuesta, HttpStatusCode.BadRequest);
    }

    [Fact]
    [Trait("HU", "HU-26")]
    public async Task Error_Conflicto_Duplicado_DebeUsarProblemDetailsConTituloEstadoDetalleCodigoYCorrelacion()
    {
        var nombre = $"Proveedor HU26 duplicado {Guid.NewGuid():N}";
        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var primera = await client.PostAsJsonAsync("/api/v1/proveedores", new { nombre });
        Assert.Equal(HttpStatusCode.Created, primera.StatusCode);

        var segunda = await client.PostAsJsonAsync("/api/v1/proveedores", new { nombre });

        await VerificarContratoProblemDetailsAsync(segunda, HttpStatusCode.Conflict);
    }

    [Fact]
    [Trait("HU", "HU-26")]
    public async Task Error_NoEncontrado_DebeUsarProblemDetailsConTituloEstadoDetalleCodigoYCorrelacion()
    {
        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var respuesta = await client.GetAsync($"/api/v1/proveedores/{Guid.NewGuid()}");

        await VerificarContratoProblemDetailsAsync(respuesta, HttpStatusCode.NotFound);
    }

    [Fact]
    [Trait("HU", "HU-26")]
    public async Task Error_Negocio_PresupuestoSuperado_DebeUsarProblemDetailsConTituloEstadoDetalleCodigoYCorrelacion()
    {
        var run = Guid.NewGuid().ToString("N")[..8];
        var fechaCierre = DateTimeOffset.UtcNow.AddDays(7);
        Guid licitacionId;
        Guid proveedorId;

        await using (var context = _database.CrearContexto())
        {
            var licitacion = Licitacion.Crear(
                $"HU26ERR{run.ToUpperInvariant()}",
                "Licitación para contrato de errores de la API",
                2_000_000m,
                fechaCierre);
            licitacion.Publicar(new LicitacionTestHelper.FixedClock(fechaCierre.AddDays(-5)));
            context.Licitaciones.Add(licitacion);
            await context.SaveChangesAsync();
            licitacionId = licitacion.Id;

            proveedorId = (await new CrearProveedorService(new ProveedorRepository(context))
                .CrearAsync(new CrearProveedorRequest($"Proveedor HU26 errores {run}"))).Id;
        }

        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var respuesta = await client.PostAsJsonAsync(
            "/api/v1/ofertas",
            new { licitacionId, proveedorId, monto = 2_000_001m });

        await VerificarContratoProblemDetailsAsync(respuesta, HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    [Trait("HU", "HU-26")]
    public async Task Error_Interno_NoControlado_DebeResponderProblemDetailsSinStackTracesNiRutasInternas()
    {
        await using var factory = CrearApiFactory(sabotearConsultaProveedores: true);
        using var client = factory.CreateClient();

        var respuesta = await client.GetAsync("/api/v1/proveedores");

        var contenido = await VerificarContratoProblemDetailsAsync(
            respuesta,
            HttpStatusCode.InternalServerError);

        foreach (var prohibido in new[]
                 {
                     "InvalidOperationException",
                     "fallo interno controlado",
                     "StackTrace",
                     "at Licitaciones",
                     "Sistema_Licitaciones"
                 })
        {
            Assert.False(
                contenido.Contains(prohibido, StringComparison.OrdinalIgnoreCase),
                $"La respuesta de error interno no debe exponer '{prohibido}'. Contenido: {contenido}");
        }
    }

    private static async Task<string> VerificarContratoProblemDetailsAsync(
        HttpResponseMessage respuesta,
        HttpStatusCode estadoEsperado)
    {
        Assert.Equal(estadoEsperado, respuesta.StatusCode);

        var tipoContenido = respuesta.Content.Headers.ContentType?.ToString();
        Assert.NotNull(tipoContenido);
        Assert.Contains("application/problem+json", tipoContenido, StringComparison.Ordinal);

        var contenido = await respuesta.Content.ReadAsStringAsync();
        using var documento = JsonDocument.Parse(contenido);
        var raiz = documento.RootElement;

        Assert.True(
            raiz.TryGetProperty("title", out var titulo)
            && titulo.ValueKind == JsonValueKind.String
            && !string.IsNullOrWhiteSpace(titulo.GetString()),
            $"El ProblemDetails debe incluir título seguro. Contenido: {contenido}");

        Assert.True(
            raiz.TryGetProperty("status", out var estado)
            && estado.TryGetInt32(out var estadoValor)
            && estadoValor == (int)estadoEsperado,
            $"El ProblemDetails debe incluir el estado {estadoEsperado}. Contenido: {contenido}");

        Assert.True(
            raiz.TryGetProperty("detail", out var detalle)
            && detalle.ValueKind == JsonValueKind.String
            && !string.IsNullOrWhiteSpace(detalle.GetString()),
            $"El ProblemDetails debe incluir un detalle seguro y comprensible. Contenido: {contenido}");

        Assert.True(
            raiz.TryGetProperty(ClaveCodigoError, out var codigo)
            && codigo.ValueKind == JsonValueKind.String
            && !string.IsNullOrWhiteSpace(codigo.GetString()),
            $"El ProblemDetails debe exponer '{ClaveCodigoError}'. Contenido: {contenido}");

        Assert.True(
            raiz.TryGetProperty(ClaveCorrelacionId, out var correlacion)
            && correlacion.ValueKind == JsonValueKind.String
            && !string.IsNullOrWhiteSpace(correlacion.GetString()),
            $"El ProblemDetails debe exponer '{ClaveCorrelacionId}'. Contenido: {contenido}");

        return contenido;
    }

    private WebApplicationFactory<Licitaciones.Api.Controllers.ProveedoresController>
        CrearApiFactory(bool sabotearConsultaProveedores = false)
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

                    if (sabotearConsultaProveedores)
                    {
                        services.RemoveAll<ConsultarProveedorService>();
                        services.AddScoped<ConsultarProveedorService>(_ =>
                            throw new InvalidOperationException("fallo interno controlado"));
                    }
                });
            });
    }
}
