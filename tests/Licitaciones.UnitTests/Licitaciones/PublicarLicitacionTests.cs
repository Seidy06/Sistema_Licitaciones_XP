using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.UnitTests.Common;
using static Licitaciones.UnitTests.Common.LicitacionTestHelper;

namespace Licitaciones.UnitTests.Licitaciones;

public sealed class PublicarLicitacionTests
{
    private static readonly DateTimeOffset Ahora =
        new(2026, 8, 18, 15, 0, 0, TimeSpan.Zero);

    [Fact]
    [Trait("HU", "HU-11")]
    public void Publicar_DesdeBorrador_DebeCambiarAPublicadaYRegistrarTransicion()
    {
        var licitacion = NuevaLicitacion(Ahora.AddDays(1));

        licitacion.Publicar(new FixedClock(Ahora));

        Assert.Equal(EstadoLicitacion.Publicada, licitacion.Estado);
        var transicion = Assert.Single(licitacion.Transiciones);
        Assert.Equal(EstadoLicitacion.Borrador, transicion.EstadoAnterior);
        Assert.Equal(EstadoLicitacion.Publicada, transicion.EstadoNuevo);
        Assert.Equal(Ahora, transicion.Fecha);
    }

    [Theory]
    [InlineData(EstadoLicitacion.Publicada)]
    [InlineData(EstadoLicitacion.Cerrada)]
    [InlineData(EstadoLicitacion.Adjudicada)]
    [InlineData(EstadoLicitacion.Cancelada)]
    [Trait("HU", "HU-11")]
    public void Publicar_DesdeEstadoDistintoDeBorrador_DebeRechazarTransicion(
        EstadoLicitacion estadoActual)
    {
        var licitacion = NuevaLicitacion(Ahora.AddDays(1));
        EstablecerEstado(licitacion, estadoActual);

        var exception = Assert.Throws<DomainException>(
            () => licitacion.Publicar(new FixedClock(Ahora)));

        Assert.Equal(
            $"No se puede publicar una licitación en estado {estadoActual}.",
            exception.Message);
        Assert.Equal(estadoActual, licitacion.Estado);
        Assert.Empty(licitacion.Transiciones);
    }

    [Fact]
    [Trait("HU", "HU-11")]
    public void Publicar_ConFechaCierrePasada_DebeRechazarPublicacion()
    {
        var licitacion = NuevaLicitacion(Ahora.AddTicks(-1));

        var exception = Assert.Throws<DomainException>(
            () => licitacion.Publicar(new FixedClock(Ahora)));

        Assert.Equal(
            "No se puede publicar una licitación cuya fecha de cierre ya pasó.",
            exception.Message);
        Assert.Equal(EstadoLicitacion.Borrador, licitacion.Estado);
        Assert.Empty(licitacion.Transiciones);
    }

    private static Licitacion NuevaLicitacion(DateTimeOffset fechaCierre) =>
        Licitacion.Crear(
            $"HU11-{Guid.NewGuid():N}",
            "Compra para pruebas HU-11",
            1000m,
            fechaCierre);
}
