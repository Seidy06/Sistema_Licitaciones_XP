using Licitaciones.Domain.Common;
using Licitaciones.Domain.Ofertas;
using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Ofertas;

public sealed class OfertaTests
{
    private readonly FixedClock _clock = new(new DateTimeOffset(2026, 8, 12, 12, 0, 0, TimeSpan.Zero));

    [Theory]
    [InlineData("0")]
    [InlineData("-0.01")]
    public void Crear_DebeRechazarMontoNoPositivo(string montoTexto)
    {
        var monto = decimal.Parse(montoTexto, System.Globalization.CultureInfo.InvariantCulture);

        Assert.Throws<DomainException>(() =>
            Oferta.Crear(Guid.NewGuid(), Guid.NewGuid(), monto, _clock));
    }

    [Fact]
    public void Crear_DebeUsarMontoDecimalYHoraDelReloj()
    {
        var oferta = Oferta.Crear(Guid.NewGuid(), Guid.NewGuid(), 1250.75m, _clock);

        Assert.Equal(1250.75m, oferta.Monto);
        Assert.Equal(_clock.UtcNowValue, oferta.FechaRegistro);
    }
}
