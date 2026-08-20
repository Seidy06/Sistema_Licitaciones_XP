using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Licitaciones;

public sealed class ConsultarLicitacionServiceTests
{
    private static readonly DateTimeOffset Ahora =
        new(2026, 8, 19, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task ListarAsync_ConFiltroEstadoPublicada_DebeRetornarSoloPublicadas()
    {
        var publicada = CrearPublicada(Ahora.AddDays(5));
        var borrador = Licitacion.Crear(
            $"HU13-{Guid.NewGuid():N}", "Borrador", 1000m, Ahora.AddDays(5));

        var repository = new RepositorioConsultaEnMemoria(
            new[] { publicada, borrador },
            new FixedClock(Ahora));
        var service = new ConsultarLicitacionService(repository);

        var resultado = await service.ListarAsync(
            new ConsultarLicitacionesRequest(
                EstadoFiltro: EstadoLicitacion.Publicada),
            new FixedClock(Ahora));

        var item = Assert.Single(resultado.Items);
        Assert.Equal(publicada.Id, item.Id);
    }

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task ListarAsync_ConFiltroEstadoCerrada_DebeIncluirCierreFuncional()
    {
        var publicadaVencida = CrearPublicada(Ahora.AddTicks(-1));
        var repository = new RepositorioConsultaEnMemoria(
            new[] { publicadaVencida },
            new FixedClock(Ahora));
        var service = new ConsultarLicitacionService(repository);

        var resultado = await service.ListarAsync(
            new ConsultarLicitacionesRequest(
                EstadoFiltro: EstadoLicitacion.Cerrada),
            new FixedClock(Ahora));

        Assert.Single(resultado.Items);
    }

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task ListarAsync_LicitacionPublicadaPeroVencida_DebeMostrarEstadoEfectivoCerrada()
    {
        var publicadaVencida = CrearPublicada(Ahora.AddTicks(-1));
        var repository = new RepositorioConsultaEnMemoria(
            new[] { publicadaVencida },
            new FixedClock(Ahora));
        var service = new ConsultarLicitacionService(repository);

        var resultado = await service.ListarAsync(
            new ConsultarLicitacionesRequest(),
            new FixedClock(Ahora));

        var item = Assert.Single(resultado.Items);
        Assert.Equal(EstadoLicitacion.Cerrada, item.EstadoEfectivo);
    }

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task ListarAsync_BorradorConCierreVencido_DebeMostrarEstadoEfectivoBorrador()
    {
        var borrador = Licitacion.Crear(
            $"HU13-{Guid.NewGuid():N}", "Borrador vencido", 1000m, Ahora.AddTicks(-1));
        var repository = new RepositorioConsultaEnMemoria(
            new[] { borrador },
            new FixedClock(Ahora));
        var service = new ConsultarLicitacionService(repository);

        var resultado = await service.ListarAsync(
            new ConsultarLicitacionesRequest(),
            new FixedClock(Ahora));

        var item = Assert.Single(resultado.Items);
        Assert.Equal(EstadoLicitacion.Borrador, item.EstadoEfectivo);
    }

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task ObtenerDetalleAsync_ConOfertas_DebeRetornarMejorOferta()
    {
        var licitacion = CrearPublicada(Ahora.AddDays(5));
        var repository = new RepositorioConsultaEnMemoria(
            new[] { licitacion },
            new FixedClock(Ahora))
        {
            MontoMinimoOferta = 8_000m
        };
        var service = new ConsultarLicitacionService(repository);

        var detalle = await service.ObtenerDetalleAsync(
            licitacion.Id, new FixedClock(Ahora));

        Assert.NotNull(detalle);
        Assert.NotNull(detalle.MejorOferta);
        Assert.Equal(8_000m, detalle.MejorOferta.Monto);
    }

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task ObtenerDetalleAsync_SinOfertas_DebeRetornarNull()
    {
        var licitacion = CrearPublicada(Ahora.AddDays(5));
        var repository = new RepositorioConsultaEnMemoria(
            new[] { licitacion },
            new FixedClock(Ahora));
        var service = new ConsultarLicitacionService(repository);

        var detalle = await service.ObtenerDetalleAsync(
            licitacion.Id, new FixedClock(Ahora));

        Assert.NotNull(detalle);
        Assert.Null(detalle.MejorOferta);
    }

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task ObtenerDetalleAsync_LicitacionInexistente_DebeRetornarNull()
    {
        var repository = new RepositorioConsultaEnMemoria(
            [],
            new FixedClock(Ahora));
        var service = new ConsultarLicitacionService(repository);

        var detalle = await service.ObtenerDetalleAsync(
            Guid.NewGuid(), new FixedClock(Ahora));

        Assert.Null(detalle);
    }

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task ObtenerDetalleAsync_MontoEntreOperativoYGerencial_DebeClasificarGerencial()
    {
        var licitacion = CrearPublicada(Ahora.AddDays(5));
        var repository = new RepositorioConsultaEnMemoria(
            new[] { licitacion },
            new FixedClock(Ahora))
        {
            MontoMinimoOferta = 5_000_000m
        };
        var service = new ConsultarLicitacionService(repository);

        var detalle = await service.ObtenerDetalleAsync(
            licitacion.Id, new FixedClock(Ahora));

        Assert.NotNull(detalle);
        Assert.NotNull(detalle.NivelAprobacion);
        Assert.Equal("Gerencial", detalle.NivelAprobacion.Nombre);
    }

    [Fact]
    [Trait("HU", "HU-13")]
    public async Task ObtenerDetalleAsync_MontoMayorOIgualADirectivo_DebeClasificarDirectivo()
    {
        var licitacion = CrearPublicada(Ahora.AddDays(5));
        var repository = new RepositorioConsultaEnMemoria(
            new[] { licitacion },
            new FixedClock(Ahora))
        {
            MontoMinimoOferta = 25_000_000m
        };
        var service = new ConsultarLicitacionService(repository);

        var detalle = await service.ObtenerDetalleAsync(
            licitacion.Id, new FixedClock(Ahora));

        Assert.NotNull(detalle);
        Assert.NotNull(detalle.NivelAprobacion);
        Assert.Equal("Directivo", detalle.NivelAprobacion.Nombre);
    }

    private static Licitacion CrearPublicada(DateTimeOffset fechaCierre)
    {
        var licitacion = Licitacion.Crear(
            $"HU13-{Guid.NewGuid():N}",
            "Compra para pruebas HU-13",
            10_000m,
            fechaCierre);

        licitacion.Publicar(new FixedClock(fechaCierre.AddDays(-5)));
        return licitacion;
    }

    private sealed class RepositorioConsultaEnMemoria : ILicitacionConsultaRepository
    {
        private readonly Licitacion[] _licitaciones;
        private readonly IClock _clock;

        public RepositorioConsultaEnMemoria(
            Licitacion[] licitaciones,
            IClock clock)
        {
            _licitaciones = licitaciones;
            _clock = clock;
        }

        public decimal? MontoMinimoOferta { get; init; }

        public Task<Licitacion?> ObtenerPorIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                _licitaciones.FirstOrDefault(l => l.Id == id));
        }

        public Task<IReadOnlyList<Licitacion>> ListarAsync(
            ConsultarLicitacionesRequest consulta,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<Licitacion> query = _licitaciones;

            if (consulta.EstadoFiltro.HasValue)
            {
                query = query.Where(l =>
                    l.EstadoEfectivo(_clock) == consulta.EstadoFiltro.Value);
            }

            return Task.FromResult<IReadOnlyList<Licitacion>>(
                query.ToArray());
        }

        public Task<decimal?> ObtenerMontoMinimoOfertaAsync(
            Guid licitacionId,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(MontoMinimoOferta);
        }

        public Task<LicitacionNivelAprobacionDto?> ObtenerNivelAprobacionAsync(
            decimal montoOferta,
            CancellationToken cancellationToken = default)
        {
            LicitacionNivelAprobacionDto? resultado = montoOferta switch
            {
                >= 10_000_000m => new LicitacionNivelAprobacionDto(3, "Directivo"),
                >= 1_000_000m => new LicitacionNivelAprobacionDto(2, "Gerencial"),
                _ => new LicitacionNivelAprobacionDto(1, "Operativo")
            };

            return Task.FromResult(resultado);
        }
    }
}
