using Licitaciones.Domain.Licitaciones;
using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Licitaciones;

public sealed class LicitacionClockTests
{
    [Fact]
    public void EstaVencida_ConFixedClock_DebeSerDeterminista()
    {
        var cierre = new DateTimeOffset(2026, 8, 12, 12, 0, 0, TimeSpan.Zero);
        var licitacion = Licitacion.Crear("LIC-001", "Compra", 1000m, cierre);
        var antesDelCierre = new FixedClock(cierre.AddTicks(-1));
        var enElCierre = new FixedClock(cierre);

        Assert.False(licitacion.EstaVencida(antesDelCierre));
        Assert.True(licitacion.EstaVencida(enElCierre));
    }
}
