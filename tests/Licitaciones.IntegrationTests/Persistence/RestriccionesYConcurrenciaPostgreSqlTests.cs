using Licitaciones.Domain.Licitaciones;
using Licitaciones.IntegrationTests.Common;
using Licitaciones.IntegrationTests.Proveedores;

using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace Licitaciones.IntegrationTests.Hu29;

[Collection(PostgreSqlCollection.Name)]
public sealed class RestriccionesYConcurrenciaPostgreSqlTests
{
    private readonly PostgreSqlFixture _database;

    public RestriccionesYConcurrenciaPostgreSqlTests(PostgreSqlFixture database) =>
        _database = database;

    [Fact]
    [Trait("HU", "HU-29")]
    public async Task Contenedor_DebeSerPostgreSqlRealConMigracionesAplicadas()
    {
        await using var contexto = _database.CrearContexto();

        Assert.True(await contexto.Database.CanConnectAsync());
        Assert.Equal(
            "Npgsql.EntityFrameworkCore.PostgreSQL",
            contexto.Database.ProviderName);

        Assert.NotEmpty(await contexto.Database.GetAppliedMigrationsAsync());
        Assert.Empty(await contexto.Database.GetPendingMigrationsAsync());

        var conexion = (NpgsqlConnection)contexto.Database.GetDbConnection();
        await contexto.Database.OpenConnectionAsync();

        Assert.StartsWith(
            "16",
            conexion.ServerVersion,
            StringComparison.Ordinal);
    }

    [Fact]
    [Trait("HU", "HU-29")]
    public async Task Insertar_CodigoDuplicadoViaEfCore_DebeRechazarloComoViolacionUnica()
    {
        var codigo = $"hu29-{Guid.NewGuid():N}";
        await using (var primerContexto = _database.CrearContexto())
        {
            primerContexto.Licitaciones.Add(NuevaLicitacion(codigo));
            await primerContexto.SaveChangesAsync();
        }

        await using var segundoContexto = _database.CrearContexto();
        segundoContexto.Licitaciones.Add(
            NuevaLicitacion($"  {codigo.ToUpperInvariant()}  "));

        var excepcion = await Assert.ThrowsAsync<DbUpdateException>(
            () => segundoContexto.SaveChangesAsync());

        var violacion = Assert.IsType<PostgresException>(excepcion.InnerException);
        Assert.Equal("23505", violacion.SqlState);
        Assert.Equal("UX_Licitaciones_CodigoNormalizado", violacion.ConstraintName);
    }

    [Fact]
    [Trait("HU", "HU-29")]
    public async Task DosActualizacionesConcurrentes_SobreMismaLicitacion_LaSegundaDebeFallar()
    {
        var codigo = $"hu29-conc-{Guid.NewGuid():N}";
        var fechaCierre = DateTimeOffset.UtcNow.AddDays(7);
        var reloj = new LicitacionTestHelper.FixedClock(fechaCierre.AddDays(-1));
        Guid licitacionId;
        await using (var contextoInicial = _database.CrearContexto())
        {
            var creada = NuevaLicitacion(codigo, fechaCierre);
            licitacionId = creada.Id;
            contextoInicial.Licitaciones.Add(creada);
            await contextoInicial.SaveChangesAsync();
        }

        await using var primerContexto = _database.CrearContexto();
        await using var segundoContexto = _database.CrearContexto();
        var desdePrimerCliente = await primerContexto.Licitaciones.SingleAsync(
            licitacion => licitacion.Id == licitacionId);
        var desdeSegundoCliente = await segundoContexto.Licitaciones.SingleAsync(
            licitacion => licitacion.Id == licitacionId);

        desdePrimerCliente.Editar(
            codigo,
            "Título del primer cliente",
            1500m,
            fechaCierre,
            reloj);
        await primerContexto.SaveChangesAsync();

        desdeSegundoCliente.Editar(
            codigo,
            "Título del segundo cliente",
            2000m,
            fechaCierre,
            reloj);

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            () => segundoContexto.SaveChangesAsync());
    }

    private static Licitacion NuevaLicitacion(string codigo) =>
        NuevaLicitacion(codigo, DateTimeOffset.UtcNow.AddDays(1));

    private static Licitacion NuevaLicitacion(string codigo, DateTimeOffset fechaCierre) =>
        Licitacion.Crear(
            codigo,
            "Compra para pruebas HU-29",
            1000m,
            fechaCierre);
}
