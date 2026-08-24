using Licitaciones.Application.Common;
using Licitaciones.Application.TiposCambio;
using Licitaciones.Domain.Common;
using Licitaciones.Domain.TiposCambio;

namespace Licitaciones.UnitTests.TiposCambio;

public sealed class AdministrarTipoCambioServiceTests
{
    private static readonly DateOnly Fecha =
        new(2026, 8, 23);

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task GuardarAsync_DebeReemplazarElActivoYRetornarDto()
    {
        var repositorio = new RepositorioFalso();
        var service = new AdministrarTipoCambioService(repositorio);

        var dto = await service.GuardarAsync(512.35m, Fecha);

        Assert.Equal("USD", dto.MonedaOrigen);
        Assert.Equal("CRC", dto.MonedaDestino);
        Assert.Equal(512.35m, dto.Valor);
        Assert.Equal(Fecha, dto.Fecha);
        Assert.True(dto.Activo);
        var reemplazado = Assert.Single(repositorio.Reemplazados);
        Assert.True(reemplazado.Activo);
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task ObtenerActivoAsync_SinTipoActivo_DebeRetornarNull()
    {
        var service = new AdministrarTipoCambioService(new RepositorioFalso());

        var dto = await service.ObtenerActivoAsync();

        Assert.Null(dto);
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task ListarAsync_DebeAplicarOrdenPorValorDescendenteYPaginacion()
    {
        var repositorio = new RepositorioFalso();
        await repositorio.PrepararAsync(
            CrearTipo(500m, Fecha.AddDays(-2)),
            CrearTipo(600m, Fecha.AddDays(-1)),
            CrearTipo(550m, Fecha));
        var service = new AdministrarTipoCambioService(repositorio);

        var pagina = await service.ListarAsync(
            ordenarPor: "valor",
            descendente: true,
            pagina: 2,
            tamanoPagina: 1);

        Assert.Equal(3, pagina.Total);
        var item = Assert.Single(pagina.Items);
        Assert.Equal(550m, item.Valor);
    }

    [Theory]
    [Trait("HU", "HU-28")]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 101)]
    public async Task ListarAsync_ConPaginacionInvalida_DebeRechazarla(
        int pagina,
        int tamanoPagina)
    {
        var service = new AdministrarTipoCambioService(new RepositorioFalso());

        await Assert.ThrowsAsync<DomainException>(() =>
            service.ListarAsync(pagina: pagina, tamanoPagina: tamanoPagina));
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task ListarAsync_ConCampoOrdenInvalido_DebeRechazarlo()
    {
        var service = new AdministrarTipoCambioService(new RepositorioFalso());

        await Assert.ThrowsAsync<DomainException>(() =>
            service.ListarAsync(ordenarPor: "moneda"));
    }

    private static TipoCambio CrearTipo(decimal valor, DateOnly fecha) =>
        TipoCambio.Crear(valor, fecha);

    private sealed class RepositorioFalso : ITipoCambioRepository
    {
        public List<TipoCambio> Reemplazados { get; } = [];

        private IReadOnlyList<TipoCambio> Tipos { get; set; } = [];

        public Task PrepararAsync(params TipoCambio[] tipos)
        {
            Tipos = tipos;
            return Task.CompletedTask;
        }

        public Task<TipoCambio?> ObtenerActivoAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<TipoCambio?>(null);

        public Task ReemplazarActivoAsync(
            TipoCambio tipoCambio,
            CancellationToken cancellationToken = default)
        {
            Reemplazados.Add(tipoCambio);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<TipoCambio>> ListarTodosAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Tipos);
    }
}
