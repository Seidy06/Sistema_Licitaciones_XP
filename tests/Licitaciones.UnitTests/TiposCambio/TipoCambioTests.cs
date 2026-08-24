using Licitaciones.Domain.Common;
using Licitaciones.Domain.TiposCambio;

namespace Licitaciones.UnitTests.TiposCambio;

public sealed class TipoCambioTests
{
    private static readonly DateOnly Fecha =
        new(2026, 8, 23);

    [Theory]
    [Trait("HU", "HU-28")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Crear_ConValorNoPositivo_DebeRechazarlo(decimal valor)
    {
        Assert.Throws<DomainException>(() => TipoCambio.Crear(valor, Fecha));
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public void Crear_NuevoTipoCambio_DebeUsarUSDaCRCYQuedarActivo()
    {
        var tipoCambio = TipoCambio.Crear(512.35m, Fecha);

        Assert.Equal("USD", tipoCambio.MonedaOrigen);
        Assert.Equal("CRC", tipoCambio.MonedaDestino);
        Assert.Equal(512.35m, tipoCambio.Valor);
        Assert.Equal(Fecha, tipoCambio.Fecha);
        Assert.True(tipoCambio.Activo);
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public void Desactivar_DebeMarcarElTipoComoInactivo()
    {
        var tipoCambio = TipoCambio.Crear(500m, Fecha);

        tipoCambio.Desactivar();

        Assert.False(tipoCambio.Activo);
    }
}
