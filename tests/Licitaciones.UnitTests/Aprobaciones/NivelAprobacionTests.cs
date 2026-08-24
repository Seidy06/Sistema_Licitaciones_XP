using Licitaciones.Domain.Aprobaciones;
using Licitaciones.Domain.Common;

namespace Licitaciones.UnitTests.Aprobaciones;

public sealed class NivelAprobacionTests
{
    [Fact]
    [Trait("HU", "HU-28")]
    public void Crear_ConNombreVacio_DebeRechazarlo()
    {
        Assert.Throws<DomainException>(
            () => NivelAprobacion.Crear("  ", 0m, null));
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public void Crear_ConMontoMinimoNegativo_DebeRechazarlo()
    {
        Assert.Throws<DomainException>(
            () => NivelAprobacion.Crear("Operativo", -1m, null));
    }

    [Theory]
    [Trait("HU", "HU-28")]
    [InlineData(100, 100)]
    [InlineData(100, 50)]
    public void Crear_ConMontoMaximoMenorOIgualAlMinimo_DebeRechazarlo(
        decimal montoMinimo,
        decimal montoMaximo)
    {
        Assert.Throws<DomainException>(
            () => NivelAprobacion.Crear("Operativo", montoMinimo, montoMaximo));
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public void Crear_NivelValidoConRangoAbierto_DebeQuedarActivoYNormalizarNombre()
    {
        var nivel = NivelAprobacion.Crear("  Operativo  ", 0m, 5_000_000m);

        Assert.Equal("Operativo", nivel.Nombre);
        Assert.Equal(0m, nivel.MontoMinimo);
        Assert.Equal(5_000_000m, nivel.MontoMaximo);
        Assert.True(nivel.Activo);
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public void Crear_NivelValidoSinMontoMaximo_DebePermitirTopeAbierto()
    {
        var nivel = NivelAprobacion.Crear("Directivo", 10_000_000m, null);

        Assert.Null(nivel.MontoMaximo);
        Assert.Equal(10_000_000m, nivel.MontoMinimo);
        Assert.True(nivel.Activo);
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public void Desactivar_DebeMarcarElNivelComoInactivo()
    {
        var nivel = NivelAprobacion.Crear("Gerencial", 5_000_000m, 10_000_000m);

        nivel.Desactivar();

        Assert.False(nivel.Activo);
    }
}
