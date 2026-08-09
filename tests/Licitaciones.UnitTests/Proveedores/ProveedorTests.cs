using Licitaciones.Domain.Proveedores;

namespace Licitaciones.UnitTests.Proveedores;

public class ProveedorTests
{
    [Fact]
    public void Crear_DebeInicializarProveedorValido()
    {
        var antes = DateTimeOffset.UtcNow;

        var proveedor = Proveedor.Crear("  Servicios   Costa Rica, S.A.  ");

        Assert.NotEqual(Guid.Empty, proveedor.Id);
        Assert.Equal("Servicios Costa Rica, S.A.", proveedor.Nombre);
        Assert.Equal("SERVICIOS COSTA RICA, S.A.", proveedor.NombreNormalizado);
        Assert.Equal(proveedor.CreatedAt, proveedor.UpdatedAt);
        Assert.InRange(proveedor.CreatedAt, antes, DateTimeOffset.UtcNow);
        Assert.Equal(0u, proveedor.Version);
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
