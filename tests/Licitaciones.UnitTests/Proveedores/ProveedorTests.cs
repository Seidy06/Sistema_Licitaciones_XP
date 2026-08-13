using Licitaciones.Domain.Proveedores;

namespace Licitaciones.UnitTests.Proveedores;

public class ProveedorTests
{
    [Fact]
    public void Crear_DebeInicializarProveedorValido()
    {
        var proveedor = Proveedor.Crear("  Servicios   Costa Rica, S.A.  ");

        Assert.NotEqual(Guid.Empty, proveedor.Id);
        Assert.Equal("Servicios Costa Rica, S.A.", proveedor.Nombre);
        Assert.Equal("SERVICIOS COSTA RICA, S.A.", proveedor.NombreNormalizado);
        Assert.Equal(default, proveedor.CreatedAt);
        Assert.Equal(default, proveedor.UpdatedAt);
        Assert.Equal(0u, proveedor.Version);
    }

    [Theory]
    [InlineData("Café Central", "Café Central")]
    [InlineData("Cafe\u0301 Central", "Café Central")]
    [InlineData(" CAFÉ   CENTRAL ", "CAFÉ CENTRAL")]
    [Trait("HU", "HU-06-Auditoria")]
    public void Crear_DebeNormalizarUnicodeAntesDeValidar(
        string nombre,
        string nombreLegibleEsperado)
    {
        var proveedor = Proveedor.Crear(nombre);

        Assert.Equal(nombreLegibleEsperado, proveedor.Nombre);
        Assert.Equal("CAFÉ CENTRAL", proveedor.NombreNormalizado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Proveedor #1")]
    public void Crear_DebeRechazarNombreInvalido(string nombre)
    {
        Assert.Throws<ArgumentException>(() => Proveedor.Crear(nombre));
    }
}
