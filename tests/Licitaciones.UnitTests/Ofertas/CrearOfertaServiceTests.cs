using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;
using Licitaciones.Domain.Proveedores;
using Licitaciones.UnitTests.Common;

using static Licitaciones.UnitTests.Common.LicitacionTestHelper;

namespace Licitaciones.UnitTests.Ofertas;

public sealed class CrearOfertaServiceTests
{
    private static readonly DateTimeOffset Ahora =
        new(2026, 8, 19, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    [Trait("HU", "HU-14")]
    public async Task CrearAsync_ConLicitacionNoPublicada_DebeRechazarOferta()
    {
        var licitacion = Licitacion.Crear(
            $"HU14-{Guid.NewGuid():N}",
            "Compra en borrador",
            10_000m,
            Ahora.AddDays(10));

        var repository = new RepositorioOfertaEnMemoria(
            licitacion,
            Proveedor.Crear("Proveedor Test"));
        var service = new CrearOfertaService(repository, new FixedClock(Ahora));

        await Assert.ThrowsAsync<DomainException>(() =>
            service.CrearAsync(new CrearOfertaRequest(
                licitacion.Id,
                Guid.NewGuid(),
                500m)));

        Assert.Null(repository.OfertaAgregada);
    }

    [Fact]
    [Trait("HU", "HU-14")]
    public async Task CrearAsync_ConLicitacionCerradaFormalmente_DebeRechazarOferta()
    {
        var licitacion = Licitacion.Crear(
            $"HU14-{Guid.NewGuid():N}",
            "Compra cerrada",
            10_000m,
            Ahora.AddDays(10));

        EstablecerEstado(licitacion, EstadoLicitacion.Cerrada);

        var repository = new RepositorioOfertaEnMemoria(
            licitacion,
            Proveedor.Crear("Proveedor Test"));
        var service = new CrearOfertaService(repository, new FixedClock(Ahora));

        await Assert.ThrowsAsync<DomainException>(() =>
            service.CrearAsync(new CrearOfertaRequest(
                licitacion.Id,
                Guid.NewGuid(),
                500m)));

        Assert.Null(repository.OfertaAgregada);
    }

    [Fact]
    [Trait("HU", "HU-14")]
    public async Task CrearAsync_ConLicitacionPublicadaVencida_DebeRechazarOferta()
    {
        var licitacion = Licitacion.Crear(
            $"HU14-{Guid.NewGuid():N}",
            "Compra vencida",
            10_000m,
            Ahora.AddTicks(-1));

        EstablecerEstado(licitacion, EstadoLicitacion.Publicada);

        var repository = new RepositorioOfertaEnMemoria(
            licitacion,
            Proveedor.Crear("Proveedor Test"));
        var service = new CrearOfertaService(repository, new FixedClock(Ahora));

        await Assert.ThrowsAsync<DomainException>(() =>
            service.CrearAsync(new CrearOfertaRequest(
                licitacion.Id,
                Guid.NewGuid(),
                500m)));

        Assert.Null(repository.OfertaAgregada);
    }

    [Fact]
    [Trait("HU", "HU-14")]
    public async Task CrearAsync_ConMontoMayorAlPresupuesto_DebeRechazarOferta()
    {
        var licitacion = Licitacion.Crear(
            $"HU14-{Guid.NewGuid():N}",
            "Compra con presupuesto limitado",
            10_000m,
            Ahora.AddDays(10));

        EstablecerEstado(licitacion, EstadoLicitacion.Publicada);

        var repository = new RepositorioOfertaEnMemoria(
            licitacion,
            Proveedor.Crear("Proveedor Test"));
        var service = new CrearOfertaService(repository, new FixedClock(Ahora));

        await Assert.ThrowsAsync<DomainException>(() =>
            service.CrearAsync(new CrearOfertaRequest(
                licitacion.Id,
                repository.ProveedorId,
                15_000m)));

        Assert.Null(repository.OfertaAgregada);
    }

    [Fact]
    [Trait("HU", "HU-14")]
    public async Task CrearAsync_ConMontoIgualAlPresupuesto_DebeAceptarOferta()
    {
        var licitacion = Licitacion.Crear(
            $"HU14-{Guid.NewGuid():N}",
            "Compra con monto exacto",
            10_000m,
            Ahora.AddDays(10));

        EstablecerEstado(licitacion, EstadoLicitacion.Publicada);

        var repository = new RepositorioOfertaEnMemoria(
            licitacion,
            Proveedor.Crear("Proveedor Test"));
        var service = new CrearOfertaService(repository, new FixedClock(Ahora));

        var resultado = await service.CrearAsync(new CrearOfertaRequest(
            licitacion.Id,
            repository.ProveedorId,
            10_000m));

        Assert.NotNull(repository.OfertaAgregada);
        Assert.Equal(10_000m, resultado.Monto);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [Trait("HU", "HU-14")]
    public async Task CrearAsync_ConMontoNoPositivo_DebeRechazarOferta(decimal monto)
    {
        var licitacion = Licitacion.Crear(
            $"HU14-{Guid.NewGuid():N}",
            "Compra para prueba de monto",
            10_000m,
            Ahora.AddDays(10));

        EstablecerEstado(licitacion, EstadoLicitacion.Publicada);

        var repository = new RepositorioOfertaEnMemoria(
            licitacion,
            Proveedor.Crear("Proveedor Test"));
        var service = new CrearOfertaService(repository, new FixedClock(Ahora));

        await Assert.ThrowsAsync<DomainException>(() =>
            service.CrearAsync(new CrearOfertaRequest(
                licitacion.Id,
                repository.ProveedorId,
                monto)));

        Assert.Null(repository.OfertaAgregada);
    }

    [Fact]
    [Trait("HU", "HU-14")]
    public async Task CrearAsync_ConProveedorDuplicado_DebeRechazarOferta()
    {
        var licitacion = Licitacion.Crear(
            $"HU14-{Guid.NewGuid():N}",
            "Compra con oferta existente",
            10_000m,
            Ahora.AddDays(10));

        EstablecerEstado(licitacion, EstadoLicitacion.Publicada);

        var repository = new RepositorioOfertaEnMemoria(
            licitacion,
            Proveedor.Crear("Proveedor Test"));
        var service = new CrearOfertaService(repository, new FixedClock(Ahora));

        await service.CrearAsync(new CrearOfertaRequest(
            licitacion.Id,
            repository.ProveedorId,
            500m));

        await Assert.ThrowsAsync<DomainException>(() =>
            service.CrearAsync(new CrearOfertaRequest(
                licitacion.Id,
                repository.ProveedorId,
                600m)));

        Assert.Single(repository.Ofertas);
    }

    private sealed class RepositorioOfertaEnMemoria : IOfertaRepository
    {
        private readonly Licitacion _licitacion;
        private readonly Proveedor _proveedor;
        private readonly List<Oferta> _ofertas = [];

        public RepositorioOfertaEnMemoria(Licitacion licitacion, Proveedor proveedor)
        {
            _licitacion = licitacion;
            _proveedor = proveedor;
        }

        public Oferta? OfertaAgregada { get; private set; }
        public IReadOnlyList<Oferta> Ofertas => _ofertas.AsReadOnly();
        public Guid ProveedorId => _proveedor.Id;

        public Task<Licitacion?> ObtenerLicitacionPorIdAsync(
            Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Licitacion?>(
                id == _licitacion.Id ? _licitacion : null);

        public Task<Proveedor?> ObtenerProveedorPorIdAsync(
            Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Proveedor?>(
                id == _proveedor.Id ? _proveedor : null);

        public Task<bool> ExisteOfertaAsync(
            Guid licitacionId, Guid proveedorId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_ofertas.Any(o =>
                o.LicitacionId == licitacionId && o.ProveedorId == proveedorId));

        public Task AgregarAsync(
            Oferta oferta, CancellationToken cancellationToken = default)
        {
            OfertaAgregada = oferta;
            _ofertas.Add(oferta);
            return Task.CompletedTask;
        }

        public Task GuardarCambiosAsync(
            CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}

public sealed record CrearOfertaRequest(
    Guid LicitacionId, Guid ProveedorId, decimal Monto);

public sealed class CrearOfertaService
{
    private readonly IOfertaRepository _repository;
    private readonly IClock _clock;

    public CrearOfertaService(IOfertaRepository repository, IClock clock)
    {
        _repository = repository;
        _clock = clock;
    }

    public Task<OfertaDto> CrearAsync(
        CrearOfertaRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public sealed class OfertaDto
{
    public Guid Id { get; init; }
    public Guid LicitacionId { get; init; }
    public Guid ProveedorId { get; init; }
    public decimal Monto { get; init; }
    public DateTimeOffset FechaRegistro { get; init; }
}

public interface IOfertaRepository
{
    Task<Licitacion?> ObtenerLicitacionPorIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    Task<Proveedor?> ObtenerProveedorPorIdAsync(
        Guid id, CancellationToken cancellationToken = default);

    Task<bool> ExisteOfertaAsync(
        Guid licitacionId, Guid proveedorId,
        CancellationToken cancellationToken = default);

    Task AgregarAsync(
        Oferta oferta, CancellationToken cancellationToken = default);

    Task GuardarCambiosAsync(
        CancellationToken cancellationToken = default);
}
