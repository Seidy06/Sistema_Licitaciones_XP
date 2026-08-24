using System.Net;
using System.Text.Json;

using Licitaciones.Infrastructure.Persistence;

using Licitaciones.IntegrationTests.Proveedores;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Licitaciones.IntegrationTests.Hu27;

[Collection(PostgreSqlCollection.Name)]
public sealed class DocumentacionSwaggerHttpTests
{
    private static readonly (string Ruta, string[] Metodos)[] EndpointsDominio =
    {
        ("/api/v1/proveedores", new[] { "get", "post" }),
        ("/api/v1/proveedores/{id}", new[] { "get", "put", "delete" }),
        ("/api/v1/proveedores/historico", new[] { "get" }),
        ("/api/v1/proveedores/historico/{id}", new[] { "get" }),
        ("/api/v1/licitaciones", new[] { "get", "post" }),
        ("/api/v1/licitaciones/{id}", new[] { "get", "put" }),
        ("/api/v1/licitaciones/{id}/publicar", new[] { "post" }),
        ("/api/v1/licitaciones/{id}/cerrar", new[] { "post" }),
        ("/api/v1/ofertas", new[] { "get", "post" }),
        ("/api/v1/ofertas/{id}", new[] { "get", "put", "delete" }),
        ("/api/v1/niveles-aprobacion", new[] { "post" }),
        ("/api/v1/niveles-aprobacion/resolver", new[] { "get" }),
        ("/api/v1/tipos-cambio", new[] { "post" }),
        ("/api/v1/tipos-cambio/activo", new[] { "get" })
    };

    private const string ClaveEsquemas = "schemas";

    private readonly PostgreSqlFixture _database;

    public DocumentacionSwaggerHttpTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-27")]
    public async Task SwaggerUi_DebeServirInterfazEnRutaSwagger()
    {
        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var respuesta = await client.GetAsync("/swagger/index.html");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
        var contenido = await respuesta.Content.ReadAsStringAsync();
        Assert.Contains(
            "swagger-ui",
            contenido,
            StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("HU", "HU-27")]
    public async Task DocumentoOpenApi_DebeExponerTodosLosEndpointsDelDominio()
    {
        var raiz = await ObtenerRaizDocumentoOpenApiAsync();

        Assert.True(
            raiz.TryGetProperty("paths", out var rutas)
            && rutas.ValueKind == JsonValueKind.Object,
            "El documento OpenAPI debe incluir la sección 'paths'.");

        foreach (var (ruta, metodos) in EndpointsDominio)
        {
            Assert.True(
                rutas.TryGetProperty(ruta, out var item)
                && item.ValueKind == JsonValueKind.Object,
                $"El documento OpenAPI debe exponer la ruta '{ruta}'.");

            foreach (var metodo in metodos)
            {
                Assert.True(
                    item.TryGetProperty(metodo, out _),
                    $"La ruta '{ruta}' debe documentar el método HTTP '{metodo.ToUpperInvariant()}'.");
            }
        }
    }

    [Fact]
    [Trait("HU", "HU-27")]
    public async Task DocumentoOpenApi_DebeIncluirEsquemasRequestResponse()
    {
        var raiz = await ObtenerRaizDocumentoOpenApiAsync();

        Assert.True(
            raiz.TryGetProperty("components", out var componentes)
            && componentes.ValueKind == JsonValueKind.Object,
            "El documento OpenAPI debe incluir la sección 'components'.");

        Assert.True(
            componentes.TryGetProperty(ClaveEsquemas, out var esquemas)
            && esquemas.ValueKind == JsonValueKind.Object,
            "El documento OpenAPI debe incluir 'components.schemas'.");

        foreach (var nombre in new[]
                 {
                     "ProveedorDto",
                     "LicitacionDto",
                     "OfertaDto",
                     "TipoCambioDto",
                     "ProblemDetails",
                     "ValidationProblemDetails"
                 })
        {
            Assert.True(
                esquemas.TryGetProperty(nombre, out _),
                $"El documento debe incluir el esquema de respuesta '{nombre}'.");
        }

        Assert.True(
            raiz.TryGetProperty("paths", out var rutas),
            "El documento OpenAPI debe incluir la sección 'paths'.");

        foreach (var (ruta, _) in EndpointsDominio)
        {
            if (!rutas.TryGetProperty(ruta, out var item))
            {
                continue;
            }

            foreach (var metodo in new[] { "post", "put" })
            {
                if (!item.TryGetProperty(metodo, out var operacion)
                    || !operacion.TryGetProperty("requestBody", out _))
                {
                    continue;
                }

                Assert.True(
                    operacion
                        .GetProperty("requestBody")
                        .GetProperty("content")
                        .TryGetProperty("application/json", out _),
                    $"La operación '{metodo.ToUpperInvariant()} {ruta}' debe documentar su cuerpo como application/json.");
            }
        }
    }

    [Fact]
    [Trait("HU", "HU-27")]
    public async Task DocumentoOpenApi_DebeIncluirEjemplos()
    {
        var contenido = await DescargarDocumentoOpenApiAsync();

        Assert.True(
            contenido.Contains("\"examples\"", StringComparison.Ordinal)
            || contenido.Contains("\"example\"", StringComparison.Ordinal),
            "El documento OpenAPI debe incluir ejemplos para esquemas u operaciones.");
    }

    private async Task<JsonElement> ObtenerRaizDocumentoOpenApiAsync()
    {
        var contenido = await DescargarDocumentoOpenApiAsync();
        using var documento = JsonDocument.Parse(contenido);
        return documento.RootElement.Clone();
    }

    private async Task<string> DescargarDocumentoOpenApiAsync()
    {
        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();

        var respuesta = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);

        var tipoContenido = respuesta.Content.Headers.ContentType?.MediaType;
        Assert.Equal("application/json", tipoContenido);

        return await respuesta.Content.ReadAsStringAsync();
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
