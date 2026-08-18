using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.UnitTests.Licitaciones;

public sealed class CrearLicitacionTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [Trait("HU", "HU-10")]
    public void Crear_ConPresupuestoNoPositivo_DebeRechazarlo(decimal presupuesto)
    {
        var exception = Assert.Throws<DomainException>(() => Licitacion.Crear(
            "LIC-2026-001",
            "Compra de equipo",
            presupuesto,
            DateTimeOffset.UtcNow.AddDays(1)));

        Assert.Equal("El presupuesto debe ser mayor que cero.", exception.Message);
    }

    [Fact]
    [Trait("HU", "HU-10")]
    public void Crear_NuevaLicitacion_DebeIniciarEnBorrador()
    {
        var licitacion = Licitacion.Crear(
            "LIC-2026-002",
            "Compra de suministros",
            1000m,
            DateTimeOffset.UtcNow.AddDays(1));

        Assert.Equal(EstadoLicitacion.Borrador, licitacion.Estado);
    }
}
