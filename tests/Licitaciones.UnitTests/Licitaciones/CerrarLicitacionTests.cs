using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Licitaciones;

public sealed class CerrarLicitacionTests
{
    private static readonly DateTimeOffset Ahora =
        new(2026, 8, 21, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    [Trait("HU", "HU-12")]
    public void Cerrar_LicitacionPublicada_RegistraTransicionACerrada()
    {
        var clock = new FixedClock(Ahora);
        var licitacion = Licitacion.Crear("HU12", "Compra", 1000m, Ahora.AddDays(1));
        licitacion.Publicar(clock);

        licitacion.Cerrar(clock);

        Assert.Equal(EstadoLicitacion.Cerrada, licitacion.Estado);
        var transicion = Assert.Single(licitacion.Transiciones, x =>
            x.EstadoNuevo == EstadoLicitacion.Cerrada);
        Assert.Equal(EstadoLicitacion.Publicada, transicion.EstadoAnterior);
        Assert.Equal(Ahora, transicion.Fecha);
    }

    [Fact]
    [Trait("HU", "HU-12")]
    public void Cerrar_LicitacionBorrador_RechazaTransicion()
    {
        var licitacion = Licitacion.Crear("HU12-B", "Compra", 1000m, Ahora.AddDays(1));

        Assert.Throws<DomainException>(() => licitacion.Cerrar(new FixedClock(Ahora)));
    }
}
