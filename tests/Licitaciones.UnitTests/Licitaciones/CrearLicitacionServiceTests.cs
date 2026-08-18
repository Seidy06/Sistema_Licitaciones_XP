using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.Licitaciones.Crear;
using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.UnitTests.Licitaciones;

public class CrearLicitacionServiceTests
{
    [Fact]
    public async Task CrearAsync_DebeAgregarLicitacionYRetornarDto()
    {
        var repository = new RepositorioEnMemoria();
        var service = new CrearLicitacionService(repository);
        var cierre = new DateTimeOffset(2026, 12, 31, 23, 59, 0, TimeSpan.Zero);

        var resultado = await service.CrearAsync(
            new CrearLicitacionRequest("  LIC-001 ", "Compra de equipos", 5_000_000m, cierre));

        Assert.Equal("LIC-001", resultado.Codigo);
        Assert.Equal("LIC-001", resultado.CodigoNormalizado);
        Assert.Equal("Compra de equipos", resultado.Titulo);
        Assert.Equal(5_000_000m, resultado.Presupuesto);
        Assert.Equal(EstadoLicitacion.Borrador, resultado.Estado);
        Assert.NotEqual(Guid.Empty, resultado.Id);
        Assert.NotNull(repository.LicitacionAgregada);
        Assert.Equal(resultado.Id, repository.LicitacionAgregada!.Id);
    }

    [Fact]
    public async Task CrearAsync_DebeRechazarCodigoDuplicado()
    {
        var repository = new RepositorioEnMemoria { Existe = true };
        var service = new CrearLicitacionService(repository);
        var cierre = new DateTimeOffset(2026, 12, 31, 23, 59, 0, TimeSpan.Zero);

        await Assert.ThrowsAsync<LicitacionDuplicadaException>(() =>
            service.CrearAsync(
                new CrearLicitacionRequest("lic-001", "Compra de equipos", 5_000_000m, cierre)));

        Assert.Equal("LIC-001", repository.CodigoConsultado);
        Assert.Null(repository.LicitacionAgregada);
    }

    [Fact]
    public async Task CrearAsync_DebeRechazarPresupuestoMenorOIgualACero()
    {
        var repository = new RepositorioEnMemoria();
        var service = new CrearLicitacionService(repository);
        var cierre = new DateTimeOffset(2026, 12, 31, 23, 59, 0, TimeSpan.Zero);

        await Assert.ThrowsAsync<DomainException>(() =>
            service.CrearAsync(
                new CrearLicitacionRequest("LIC-001", "Compra", 0m, cierre)));

        await Assert.ThrowsAsync<DomainException>(() =>
            service.CrearAsync(
                new CrearLicitacionRequest("LIC-002", "Compra", -100m, cierre)));

        Assert.Null(repository.LicitacionAgregada);
    }

    [Fact]
    public async Task CrearAsync_DebeNormalizarCodigo()
    {
        var repository = new RepositorioEnMemoria();
        var service = new CrearLicitacionService(repository);
        var cierre = new DateTimeOffset(2026, 12, 31, 23, 59, 0, TimeSpan.Zero);

        var resultado = await service.CrearAsync(
            new CrearLicitacionRequest("  lic-002  ", "Servicios", 1_000_000m, cierre));

        Assert.Equal("lic-002", resultado.Codigo);
        Assert.Equal("LIC-002", resultado.CodigoNormalizado);
    }

    private sealed class RepositorioEnMemoria : ILicitacionRepository
    {
        public bool Existe { get; init; }

        public string? CodigoConsultado { get; private set; }

        public Licitacion? LicitacionAgregada { get; private set; }

        public Task<bool> ExisteCodigoNormalizadoAsync(
            string codigoNormalizado,
            CancellationToken cancellationToken = default)
        {
            CodigoConsultado = codigoNormalizado;
            return Task.FromResult(Existe);
        }

        public Task AgregarAsync(
            Licitacion licitacion,
            CancellationToken cancellationToken = default)
        {
            LicitacionAgregada = licitacion;
            return Task.CompletedTask;
        }
    }
}
