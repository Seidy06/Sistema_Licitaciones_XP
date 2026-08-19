using Licitaciones.Application.Licitaciones;
using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;
using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Licitaciones;

public sealed class EditarLicitacionServiceTests
{
    private static readonly DateTimeOffset Ahora =
        new(2026, 8, 19, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    [Trait("HU", "HU-12")]
    public async Task Editar_PresupuestoPorDebajoDeOfertaExistente_DebeRechazar()
    {
        var licitacionId = Guid.NewGuid();
        var licitacion = Licitacion.Crear(
            $"HU12-{Guid.NewGuid():N}",
            "Compra con oferta",
            10_000m,
            Ahora.AddDays(10));

        EstablecerEstado(licitacion, EstadoLicitacion.Publicada);

        var repository = new RepositorioEnMemoria(licitacion)
        {
            MontoMinimoOferta = 5_000m
        };
        var service = new EditarLicitacionService(repository, new FixedClock(Ahora));

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            service.EditarAsync(new EditarLicitacionRequest(
                licitacionId,
                Codigo: null,
                Titulo: null,
                Presupuesto: 4_000m,
                FechaCierre: null)));

        Assert.Contains("presupuesto", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("HU", "HU-12")]
    public async Task Editar_CodigoEnLicitacionCerrada_DebeRechazar()
    {
        var licitacion = Licitacion.Crear(
            $"HU12-{Guid.NewGuid():N}",
            "Compra cerrada formal",
            10_000m,
            Ahora.AddDays(10));

        EstablecerEstado(licitacion, EstadoLicitacion.Cerrada);

        var repository = new RepositorioEnMemoria(licitacion);
        var service = new EditarLicitacionService(repository, new FixedClock(Ahora));

        await Assert.ThrowsAsync<DomainException>(() =>
            service.EditarAsync(new EditarLicitacionRequest(
                licitacion.Id,
                Codigo: "LIC-NUEVO",
                Titulo: null,
                Presupuesto: null,
                FechaCierre: null)));
    }

    [Fact]
    [Trait("HU", "HU-12")]
    public async Task Editar_PresupuestoEnLicitacionCerrada_DebeRechazar()
    {
        var licitacion = Licitacion.Crear(
            $"HU12-{Guid.NewGuid():N}",
            "Compra cerrada formal",
            10_000m,
            Ahora.AddDays(10));

        EstablecerEstado(licitacion, EstadoLicitacion.Cerrada);

        var repository = new RepositorioEnMemoria(licitacion);
        var service = new EditarLicitacionService(repository, new FixedClock(Ahora));

        await Assert.ThrowsAsync<DomainException>(() =>
            service.EditarAsync(new EditarLicitacionRequest(
                licitacion.Id,
                Codigo: null,
                Titulo: null,
                Presupuesto: 20_000m,
                FechaCierre: null)));
    }

    [Fact]
    [Trait("HU", "HU-12")]
    public async Task Editar_FechaCierreEnLicitacionCerrada_DebeRechazar()
    {
        var licitacion = Licitacion.Crear(
            $"HU12-{Guid.NewGuid():N}",
            "Compra cerrada formal",
            10_000m,
            Ahora.AddDays(10));

        EstablecerEstado(licitacion, EstadoLicitacion.Cerrada);

        var repository = new RepositorioEnMemoria(licitacion);
        var service = new EditarLicitacionService(repository, new FixedClock(Ahora));

        await Assert.ThrowsAsync<DomainException>(() =>
            service.EditarAsync(new EditarLicitacionRequest(
                licitacion.Id,
                Codigo: null,
                Titulo: null,
                Presupuesto: null,
                FechaCierre: Ahora.AddDays(30))));
    }

    [Fact]
    [Trait("HU", "HU-12")]
    public async Task Editar_CampoProtegidoEnLicitacionCerradaFuncionalmente_DebeRechazar()
    {
        var licitacion = Licitacion.Crear(
            $"HU12-{Guid.NewGuid():N}",
            "Compra cerrada funcional",
            10_000m,
            Ahora.AddTicks(-1));

        EstablecerEstado(licitacion, EstadoLicitacion.Publicada);

        var repository = new RepositorioEnMemoria(licitacion);
        var service = new EditarLicitacionService(repository, new FixedClock(Ahora));

        await Assert.ThrowsAsync<DomainException>(() =>
            service.EditarAsync(new EditarLicitacionRequest(
                licitacion.Id,
                Codigo: null,
                Titulo: null,
                Presupuesto: 20_000m,
                FechaCierre: null)));
    }

    [Fact]
    [Trait("HU", "HU-12")]
    public async Task Editar_TituloEnLicitacionPublicada_DebePermitir()
    {
        var licitacion = Licitacion.Crear(
            $"HU12-{Guid.NewGuid():N}",
            "Compra abierta",
            10_000m,
            Ahora.AddDays(10));

        EstablecerEstado(licitacion, EstadoLicitacion.Publicada);

        var repository = new RepositorioEnMemoria(licitacion);
        var service = new EditarLicitacionService(repository, new FixedClock(Ahora));

        var resultado = await service.EditarAsync(new EditarLicitacionRequest(
            licitacion.Id,
            Codigo: null,
            Titulo: "Titulo actualizado",
            Presupuesto: null,
            FechaCierre: null));

        Assert.Equal("Titulo actualizado", resultado.Titulo);
    }

    private static void EstablecerEstado(
        Licitacion licitacion,
        EstadoLicitacion estado)
    {
        typeof(Licitacion)
            .GetProperty(
                nameof(Licitacion.Estado),
                System.Reflection.BindingFlags.Instance
                    | System.Reflection.BindingFlags.Public)!
            .SetValue(licitacion, estado);
    }

    private sealed class RepositorioEnMemoria : ILicitacionRepository
    {
        private readonly Licitacion _licitacion;

        public RepositorioEnMemoria(Licitacion licitacion)
        {
            _licitacion = licitacion;
        }

        public decimal? MontoMinimoOferta { get; init; }

        public Task<Licitacion?> ObtenerPorIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<Licitacion?>(
                _licitacion.Id == id ? _licitacion : null);
        }

        public Task<decimal?> ObtenerMontoMinimoOfertaAsync(
            Guid licitacionId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(MontoMinimoOferta);
        }

        public Task<bool> ExisteCodigoNormalizadoAsync(
            string codigoNormalizado,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task AgregarAsync(
            Licitacion licitacion,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task GuardarCambiosAsync(
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    public sealed record EditarLicitacionRequest(
        Guid Id,
        string? Codigo,
        string? Titulo,
        decimal? Presupuesto,
        DateTimeOffset? FechaCierre);
}
