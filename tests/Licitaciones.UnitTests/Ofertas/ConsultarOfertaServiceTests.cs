using Licitaciones.Application.Common;
using Licitaciones.Application.Ofertas.Consultar;
using Licitaciones.Application.TiposCambio;
using Licitaciones.Domain.Common;
using Licitaciones.Domain.TiposCambio;
using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Ofertas;

public sealed class ConsultarOfertaServiceTests
{
    private static readonly DateTimeOffset Ahora =
        new(2026, 8, 23, 8, 0, 0, TimeSpan.Zero);

    private static readonly DateOnly FechaTipoCambio =
        new(2026, 8, 22);

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task ListarAsync_ConMonedaUSD_DebeConvertirMontosConTipoCambioActivo()
    {
        var licitacionId = Guid.NewGuid();
        var ofertaMenor = CrearRegistro(licitacionId, "Soltec", 10_000m, Ahora);
        var ofertaMayor = CrearRegistro(licitacionId, "Importk", 50_000m, Ahora);
        var service = CrearService(
            [ofertaMenor, ofertaMayor],
            tipoCambioActivo: TipoCambio.Crear(500m, FechaTipoCambio));

        var pagina = await service.ListarAsync(new ConsultarOfertasRequest(
            licitacionId,
            Moneda: "usd"));

        Assert.All(pagina.Items, item => Assert.Equal("USD", item.Moneda));
        var convertidaMenor = pagina.Items.Single(
            x => x.Id == ofertaMenor.Id);
        Assert.Equal(20m, convertidaMenor.Monto);
        Assert.Equal(500m, convertidaMenor.TipoCambioValor);
        Assert.Equal(FechaTipoCambio, convertidaMenor.TipoCambioFecha);
        var convertidaMayor = pagina.Items.Single(
            x => x.Id == ofertaMayor.Id);
        Assert.Equal(100m, convertidaMayor.Monto);
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task ListarAsync_DebeMarcarLaMejorOfertaPorMontoYAntiguedad()
    {
        var licitacionId = Guid.NewGuid();
        var empatePrimero = CrearRegistro(licitacionId, "Soltec", 9_000m, Ahora.AddMinutes(1));
        var empateSegundo = CrearRegistro(licitacionId, "Importk", 9_000m, Ahora.AddMinutes(2));
        var mayor = CrearRegistro(licitacionId, "TecnoCR", 12_000m, Ahora);
        var service = CrearService([mayor, empateSegundo, empatePrimero]);

        var pagina = await service.ListarAsync(new ConsultarOfertasRequest(licitacionId));

        Assert.True(pagina.Items.Single(x => x.Id == empatePrimero.Id).EsMejorOferta);
        Assert.False(pagina.Items.Single(x => x.Id == empateSegundo.Id).EsMejorOferta);
        Assert.False(pagina.Items.Single(x => x.Id == mayor.Id).EsMejorOferta);
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task ListarAsync_ConMonedaCRC_NoDebeRequerirTipoCambioNiConvertir()
    {
        var licitacionId = Guid.NewGuid();
        var oferta = CrearRegistro(licitacionId, "Soltec", 10_000m, Ahora);
        var service = CrearService([oferta], tipoCambioActivo: null);

        var pagina = await service.ListarAsync(new ConsultarOfertasRequest(
            licitacionId,
            Moneda: "crc"));

        var item = Assert.Single(pagina.Items);
        Assert.Equal(10_000m, item.Monto);
        Assert.Null(item.TipoCambioValor);
        Assert.Null(item.TipoCambioFecha);
    }

    [Theory]
    [Trait("HU", "HU-28")]
    [InlineData("EUR")]
    [InlineData("   ")]
    public async Task ListarAsync_ConMonedaNoSoportada_DebeRechazarla(string moneda)
    {
        var service = CrearService([]);

        await Assert.ThrowsAsync<DomainException>(
            () => service.ListarAsync(
                new ConsultarOfertasRequest(Guid.NewGuid(), Moneda: moneda)));
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task ListarAsync_ConMonedaUSDSinTipoCambioActivo_DebeRechazar()
    {
        var service = CrearService([], tipoCambioActivo: null);

        await Assert.ThrowsAsync<DomainException>(
            () => service.ListarAsync(new ConsultarOfertasRequest(
                Guid.NewGuid(),
                Moneda: "USD")));
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task ListarAsync_SinLicitacion_DebeRechazarConsulta()
    {
        var service = CrearService([]);

        await Assert.ThrowsAsync<DomainException>(
            () => service.ListarAsync(new ConsultarOfertasRequest(Guid.Empty)));
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task ListarAsync_ConFiltroProveedor_DebeAplicarCoincidenciaParcial()
    {
        var licitacionId = Guid.NewGuid();
        var soltec = CrearRegistro(licitacionId, "Soltec SA", 10_000m, Ahora);
        var importk = CrearRegistro(licitacionId, "Importk Ltda", 11_000m, Ahora);
        var service = CrearService([soltec, importk]);

        var pagina = await service.ListarAsync(new ConsultarOfertasRequest(
            licitacionId,
            Proveedor: "SOL"));

        var item = Assert.Single(pagina.Items);
        Assert.Equal("Soltec SA", item.ProveedorNombre);
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task ObtenerAsync_OfertaInexistente_DebeRetornarNull()
    {
        var service = CrearService([]);

        var dto = await service.ObtenerAsync(Guid.NewGuid(), "CRC");

        Assert.Null(dto);
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task ListarAsync_ConOrdenPorMontoDescendenteYPaginacion_DebeAplicarlo()
    {
        var licitacionId = Guid.NewGuid();
        var ofertas = new[]
        {
            CrearRegistro(licitacionId, "A", 10_000m, Ahora),
            CrearRegistro(licitacionId, "B", 30_000m, Ahora),
            CrearRegistro(licitacionId, "C", 20_000m, Ahora)
        };
        var service = CrearService(ofertas);

        var pagina = await service.ListarAsync(new ConsultarOfertasRequest(
            licitacionId,
            OrdenarPor: "monto",
            Descendente: true,
            Pagina: 2,
            TamanoPagina: 2));

        Assert.Equal(3, pagina.Total);
        var item = Assert.Single(pagina.Items);
        Assert.Equal(10_000m, item.Monto);
    }

    private static OfertaConsultaRegistro CrearRegistro(
        Guid licitacionId,
        string proveedor,
        decimal monto,
        DateTimeOffset fecha) =>
        new(Guid.NewGuid(), licitacionId, proveedor, monto, fecha);

    private static ConsultarOfertaService CrearService(
        IReadOnlyList<OfertaConsultaRegistro> ofertas,
        TipoCambio? tipoCambioActivo = null)
    {
        TipoCambio[] tipos = tipoCambioActivo is null ? [] : [tipoCambioActivo];
        return new(
            new RepositorioConsultaFalso(ofertas),
            new RepositorioTipoCambioEnMemoria(tipos));
    }

    private sealed class RepositorioConsultaFalso : IOfertaConsultaRepository
    {
        private readonly IReadOnlyList<OfertaConsultaRegistro> _ofertas;

        public RepositorioConsultaFalso(IReadOnlyList<OfertaConsultaRegistro> ofertas) =>
            _ofertas = ofertas;

        public Task<IReadOnlyList<OfertaConsultaRegistro>> ListarAsync(
            Guid licitacionId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_ofertas);

        public Task<OfertaConsultaRegistro?> ObtenerPorIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_ofertas.FirstOrDefault(x => x.Id == id));
    }
}
