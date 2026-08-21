using System.Net;
using System.Net.Http.Json;

using Licitaciones.Application.Proveedores;
using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;
using Licitaciones.Infrastructure.Persistence;
using Licitaciones.Infrastructure.Time;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Licitaciones.IntegrationTests.Proveedores;

[Collection(PostgreSqlCollection.Name)]
public sealed class ProveedorEndToEndHttpTests
{
    private readonly PostgreSqlFixture _database;

    public ProveedorEndToEndHttpTests(PostgreSqlFixture database)
    {
        _database = database;
    }

    [Fact]
    [Trait("HU", "HU-06-HU-09")]
    public async Task Api_CRUDLogicoCompleto_DebeAtravesarPipelineHttpYPersistir()
    {
        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();
        var nombre = $"Proveedor E2E API {Guid.NewGuid():N}";

        var post = await client.PostAsJsonAsync(
            "/api/v1/proveedores",
            new { nombre });

        Assert.Equal(HttpStatusCode.Created, post.StatusCode);
        var creado = await post.Content.ReadFromJsonAsync<ProveedorDto>();
        Assert.NotNull(creado);

        var listado = await client.GetFromJsonAsync<PaginaResultado<ProveedorDto>>(
            $"/api/v1/proveedores?nombre={Uri.EscapeDataString(nombre)}");
        Assert.Contains(listado!.Items, proveedor => proveedor.Id == creado.Id);

        var detalle = await client.GetAsync($"/api/v1/proveedores/{creado.Id}");
        Assert.Equal(HttpStatusCode.OK, detalle.StatusCode);

        var nombreEditado = $"Proveedor E2E editado {Guid.NewGuid():N}";
        var put = await client.PutAsJsonAsync(
            $"/api/v1/proveedores/{creado.Id}",
            new { nombre = nombreEditado, version = creado.Version });
        Assert.Equal(HttpStatusCode.OK, put.StatusCode);

        var delete = await client.DeleteAsync($"/api/v1/proveedores/{creado.Id}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        Assert.Equal(
            HttpStatusCode.NotFound,
            (await client.GetAsync($"/api/v1/proveedores/{creado.Id}")).StatusCode);

        var historico = await client.GetFromJsonAsync<ProveedorHistoricoDto>(
            $"/api/v1/proveedores/historico/{creado.Id}");
        Assert.NotNull(historico);
        Assert.Equal(nombreEditado, historico.Nombre);
        Assert.NotEqual(default, historico.DeletedAt);
    }

    [Fact]
    [Trait("HU", "HU-06")]
    public async Task Api_NombreDuplicado_DebeResponderConflictDesdeHttpReal()
    {
        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();
        var nombre = $"Café HTTP real {Guid.NewGuid():N}";

        var primera = await client.PostAsJsonAsync(
            "/api/v1/proveedores",
            new { nombre = nombre.Normalize(System.Text.NormalizationForm.FormD) });
        var segunda = await client.PostAsJsonAsync(
            "/api/v1/proveedores",
            new { nombre = $"  {nombre.ToUpperInvariant()}  " });

        Assert.Equal(HttpStatusCode.Created, primera.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, segunda.StatusCode);
    }

    [Fact]
    [Trait("HU", "HU-08")]
    public async Task Api_ProveedorConOferta_DebeDarBajaSinEliminarRelacionesHistoricas()
    {
        await using var factory = CrearApiFactory();
        using var client = factory.CreateClient();
        var post = await client.PostAsJsonAsync(
            "/api/v1/proveedores",
            new { nombre = $"Proveedor con oferta {Guid.NewGuid():N}" });
        var proveedor = await post.Content.ReadFromJsonAsync<ProveedorDto>();
        Assert.NotNull(proveedor);

        Guid ofertaId;
        await using (var context = _database.CrearContexto())
        {
            var ahora = DateTimeOffset.UtcNow;
            var licitacion = Licitacion.Crear(
                $"HIST-{Guid.NewGuid():N}",
                "Licitación histórica",
                1000m,
                ahora.AddDays(1));
            var oferta = Oferta.Crear(
                licitacion.Id,
                proveedor.Id,
                500m,
                new SystemClock());
            context.AddRange(licitacion, oferta);
            await context.SaveChangesAsync();
            ofertaId = oferta.Id;
        }

        var delete = await client.DeleteAsync($"/api/v1/proveedores/{proveedor.Id}");

        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
        await using var verificationContext = _database.CrearContexto();
        Assert.True(await verificationContext.Ofertas.AnyAsync(oferta => oferta.Id == ofertaId));
        Assert.True(await verificationContext.Proveedores
            .IgnoreQueryFilters()
            .AnyAsync(item => item.Id == proveedor.Id && item.DeletedAt != null));
    }

    [Fact]
    [Trait("HU", "HU-06-HU-09")]
    public async Task Mvc_CRUDLogicoCompleto_DebeAtravesarPipelineHttpYMostrarHistorico()
    {
        await using var factory = CrearWebFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        var nombre = $"Proveedor E2E MVC {Guid.NewGuid():N}";

        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/Proveedores")).StatusCode);

        var create = await client.PostAsync(
            "/Proveedores/Create",
            new FormUrlEncodedContent(new Dictionary<string, string> { ["Nombre"] = nombre }));
        Assert.Equal(HttpStatusCode.Redirect, create.StatusCode);

        Guid id;
        uint version;
        await using (var context = _database.CrearContexto())
        {
            var proveedor = await context.Proveedores
                .AsNoTracking()
                .SingleAsync(item => item.Nombre == nombre);
            id = proveedor.Id;
            version = proveedor.Version;
        }

        var nombreEditado = $"Proveedor MVC editado {Guid.NewGuid():N}";
        var edit = await client.PostAsync(
            $"/Proveedores/Edit/{id}",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Id"] = id.ToString(),
                ["Nombre"] = nombreEditado,
                ["Version"] = version.ToString()
            }));
        Assert.Equal(HttpStatusCode.Redirect, edit.StatusCode);

        var details = await client.GetStringAsync($"/Proveedores/Details/{id}");
        Assert.Contains(nombreEditado, details, StringComparison.Ordinal);

        var delete = await client.PostAsync(
            $"/Proveedores/DeleteConfirmed/{id}",
            new FormUrlEncodedContent([]));
        Assert.Equal(HttpStatusCode.Redirect, delete.StatusCode);

        var history = await client.GetStringAsync("/Proveedores/History");
        Assert.Contains(nombreEditado, history, StringComparison.Ordinal);
        Assert.Equal(
            HttpStatusCode.OK,
            (await client.GetAsync($"/Proveedores/HistoryDetails/{id}")).StatusCode);
    }

    private WebApplicationFactory<Licitaciones.Api.Controllers.ProveedoresController>
        CrearApiFactory()
    {
        return new WebApplicationFactory<Licitaciones.Api.Controllers.ProveedoresController>()
            .WithWebHostBuilder(builder => ConfigurarPostgreSql(builder, aplicarMigraciones: true));
    }

    private WebApplicationFactory<Licitaciones.Web.Controllers.ProveedoresController>
        CrearWebFactory()
    {
        return new WebApplicationFactory<Licitaciones.Web.Controllers.ProveedoresController>()
            .WithWebHostBuilder(builder =>
            {
                ConfigurarPostgreSql(builder, aplicarMigraciones: false);
                builder.ConfigureServices(services =>
                    services.AddControllersWithViews(options =>
                        options.Filters.Add(
                            new Microsoft.AspNetCore.Mvc.IgnoreAntiforgeryTokenAttribute
                            {
                                Order = 1001
                            })));
            });
    }

    private void ConfigurarPostgreSql(IWebHostBuilder builder, bool aplicarMigraciones)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:ApplyMigrationsOnStartup"] = aplicarMigraciones.ToString()
            }));
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
        });
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<LicitacionesDbContext>>();
            services.RemoveAll<LicitacionesDbContext>();
            services.AddDbContext<LicitacionesDbContext>(options =>
                options.UseNpgsql(_database.ConnectionString));
            services.AddDataProtection().UseEphemeralDataProtectionProvider();
        });
    }
}
