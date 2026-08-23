using System.Net;
using System.Text.RegularExpressions;

using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Domain.Aprobaciones;
using Licitaciones.Infrastructure.Persistence;
using Licitaciones.IntegrationTests.Proveedores;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Licitaciones.IntegrationTests.Hu23;

[Collection(PostgreSqlCollection.Name)]
public sealed class CrudWebConfirmacionEliminacionTests
{
    private readonly PostgreSqlFixture _database;

    public CrudWebConfirmacionEliminacionTests(PostgreSqlFixture database) => _database = database;

    [Fact]
    [Trait("HU", "HU-23")]
    public async Task Eliminacion_Proveedores_DebePedirConfirmacionAntesDeEjecutar()
    {
        var run = Guid.NewGuid().ToString("N")[..8];
        var creado = await CrearProveedorAsync($"HU23{run} baja confirmada");

        await using var factory = CrearWebFactory();
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var confirmacion = await client.GetAsync($"/Proveedores/Delete/{creado.Id}");
        Assert.True(confirmacion.IsSuccessStatusCode,
            "Solicitar la eliminación debe mostrar una vista de confirmación (200).");
        var htmlConfirmacion = await confirmacion.Content.ReadAsStringAsync();
        Assert.Contains("Confirmar", htmlConfirmacion, StringComparison.Ordinal);
        Assert.Contains(
            "DeleteConfirmed",
            htmlConfirmacion,
            StringComparison.Ordinal);

        await using (var contextoVerificacion = _database.CrearContexto())
        {
            var pendiente = await contextoVerificacion.Proveedores
                .IgnoreQueryFilters()
                .SingleAsync(proveedor => proveedor.Id == creado.Id);
            Assert.Null(pendiente.DeletedAt);
        }

        var ejecucion = await client.PostAsync(
            $"/Proveedores/DeleteConfirmed/{creado.Id}",
            new FormUrlEncodedContent([]));

        Assert.Equal(HttpStatusCode.Redirect, ejecucion.StatusCode);

        await using var contextoFinal = _database.CrearContexto();
        var eliminado = await contextoFinal.Proveedores
            .IgnoreQueryFilters()
            .SingleAsync(proveedor => proveedor.Id == creado.Id);
        Assert.NotNull(eliminado.DeletedAt);
    }

    [Fact]
    [Trait("HU", "HU-23")]
    public async Task Eliminacion_NivelesAprobacion_DebePedirConfirmacionAntesDeEjecutar()
    {
        var run = Guid.NewGuid().ToString("N")[..8];
        int nivelId = 0;
        try
        {
            nivelId = await SembrarNivelEliminableAsync(run);

            await using var factory = CrearWebFactory();
            using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            var confirmacion = await client.GetAsync($"/NivelesAprobacion/Delete/{nivelId}");
            Assert.True(confirmacion.IsSuccessStatusCode,
                "Solicitar la eliminación del nivel debe mostrar una vista de confirmación (200).");
            var htmlConfirmacion = await confirmacion.Content.ReadAsStringAsync();
            Assert.Contains("Confirmar", htmlConfirmacion, StringComparison.Ordinal);
            Assert.Matches(
                "action=\"/NivelesAprobacion/DeleteConfirmed/[^\"]*\"",
                htmlConfirmacion);

            await using (var contextoVerificacion = _database.CrearContexto())
            {
                var pendiente = await contextoVerificacion.NivelesAprobacion
                    .SingleAsync(nivel => nivel.Id == nivelId);
                Assert.True(pendiente.Activo,
                    "La vista de confirmación no debe desactivar el nivel todavía.");
            }

            var ejecucion = await client.PostAsync(
                $"/NivelesAprobacion/DeleteConfirmed/{nivelId}",
                new FormUrlEncodedContent([]));

            Assert.True(
                ejecucion.StatusCode is HttpStatusCode.Redirect or HttpStatusCode.OK,
                "Tras confirmar, la eliminación debe completarse.");

            await using var contextoFinal = _database.CrearContexto();
            var eliminado = await contextoFinal.NivelesAprobacion
                .SingleAsync(nivel => nivel.Id == nivelId);
            Assert.False(eliminado.Activo,
                "Tras la confirmación el nivel debe quedar desactivado.");
        }
        finally
        {
            await RestaurarCatalogoNivelesAsync(nivelId);
        }
    }

    private async Task<int> SembrarNivelEliminableAsync(string run)
    {
        await using var context = _database.CrearContexto();

        await context.Database.ExecuteSqlRawAsync(
            "UPDATE \"NivelesAprobacion\" SET \"Activo\" = {0} WHERE \"Id\" = {1}",
            false, 3);

        var nivel = NivelAprobacion.Crear(
            $"Nivel eliminable HU23{run}",
            11_000_000m,
            12_000_000m);
        context.NivelesAprobacion.Add(nivel);
        await context.SaveChangesAsync();
        return nivel.Id;
    }

    private async Task RestaurarCatalogoNivelesAsync(int nivelId)
    {
        await using var context = _database.CrearContexto();
        if (nivelId > 0)
        {
            await context.Database.ExecuteSqlRawAsync(
                "DELETE FROM \"NivelesAprobacion\" WHERE \"Id\" = {0}",
                nivelId);
        }

        await context.Database.ExecuteSqlRawAsync(
            "UPDATE \"NivelesAprobacion\" SET \"Activo\" = {0} WHERE \"Id\" = {1}",
            true, 3);
    }

    private async Task<Licitaciones.Application.Proveedores.ProveedorDto> CrearProveedorAsync(
        string nombre)
    {
        await using var context = _database.CrearContexto();
        return await new CrearProveedorService(new ProveedorRepository(context))
            .CrearAsync(new Licitaciones.Application.Proveedores.Crear.CrearProveedorRequest(nombre));
    }

    private WebApplicationFactory<Licitaciones.Web.Controllers.ProveedoresController>
        CrearWebFactory()
    {
        return new WebApplicationFactory<Licitaciones.Web.Controllers.ProveedoresController>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, configuration) =>
                    configuration.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        ["Database:ApplyMigrationsOnStartup"] = "false"
                    }));
                builder.ConfigureLogging(logging => logging.ClearProviders());
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<LicitacionesDbContext>>();
                    services.RemoveAll<LicitacionesDbContext>();
                    services.AddControllersWithViews(options =>
                        options.Filters.Add(
                            new Microsoft.AspNetCore.Mvc.IgnoreAntiforgeryTokenAttribute
                            {
                                Order = 1001
                            }));
                    services.AddDbContext<LicitacionesDbContext>(options =>
                        options.UseNpgsql(_database.ConnectionString));
                    services.AddDataProtection().UseEphemeralDataProtectionProvider();
                });
            });
    }
}
