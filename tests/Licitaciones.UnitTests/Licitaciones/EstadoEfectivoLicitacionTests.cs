using System.Reflection;

using Licitaciones.Domain.Licitaciones;
using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Licitaciones;

public sealed class EstadoEfectivoLicitacionTests
{
    private static readonly DateTimeOffset Ahora =
        new(2026, 8, 19, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    [Trait("HU", "HU-12")]
    public void EstadoEfectivo_PublicadaConFechaCierreAlcanzada_DebeRetornarCerrada()
    {
        var licitacion = Licitacion.Crear(
            $"HU12-{Guid.NewGuid():N}",
            "Compra con cierre vencido",
            1000m,
            Ahora.AddTicks(-1));

        EstablecerEstado(licitacion, EstadoLicitacion.Publicada);

        var estadoEfectivo = licitacion.EstadoEfectivo(new FixedClock(Ahora));

        Assert.Equal(EstadoLicitacion.Cerrada, estadoEfectivo);
    }

    [Fact]
    [Trait("HU", "HU-12")]
    public void EstadoEfectivo_PublicadaSinFechaCierreAlcanzada_DebeRetornarPublicada()
    {
        var licitacion = Licitacion.Crear(
            $"HU12-{Guid.NewGuid():N}",
            "Compra con cierre futuro",
            1000m,
            Ahora.AddDays(1));

        EstablecerEstado(licitacion, EstadoLicitacion.Publicada);

        var estadoEfectivo = licitacion.EstadoEfectivo(new FixedClock(Ahora));

        Assert.Equal(EstadoLicitacion.Publicada, estadoEfectivo);
    }

    [Fact]
    [Trait("HU", "HU-12")]
    public void EstadoEfectivo_BorradorConFechaCierreAlcanzada_DebeRetornarBorrador()
    {
        var licitacion = Licitacion.Crear(
            $"HU12-{Guid.NewGuid():N}",
            "Borrador con cierre vencido",
            1000m,
            Ahora.AddTicks(-1));

        var estadoEfectivo = licitacion.EstadoEfectivo(new FixedClock(Ahora));

        Assert.Equal(EstadoLicitacion.Borrador, estadoEfectivo);
    }

    [Fact]
    [Trait("HU", "HU-12")]
    public void EstadoEfectivo_CerradaFormalmente_DebeRetornarCerrada()
    {
        var licitacion = Licitacion.Crear(
            $"HU12-{Guid.NewGuid():N}",
            "Cerrada formal",
            1000m,
            Ahora.AddDays(1));

        EstablecerEstado(licitacion, EstadoLicitacion.Cerrada);

        var estadoEfectivo = licitacion.EstadoEfectivo(new FixedClock(Ahora));

        Assert.Equal(EstadoLicitacion.Cerrada, estadoEfectivo);
    }

    private static void EstablecerEstado(
        Licitacion licitacion,
        EstadoLicitacion estado)
    {
        typeof(Licitacion)
            .GetProperty(
                nameof(Licitacion.Estado),
                BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(licitacion, estado);
    }
}
