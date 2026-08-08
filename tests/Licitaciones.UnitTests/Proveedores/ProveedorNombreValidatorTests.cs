using Licitaciones.Domain.Proveedores;
using Xunit;

namespace Licitaciones.UnitTests.Proveedores;

public class ProveedorNombreValidatorTests
{
    [Theory]
    [InlineData("Empresa Central")]
    [InlineData("Proveedor 123")]
    [InlineData("Servicios (Costa Rica), S.A.")]
    public void EsValido_DebeAceptarCaracteresPermitidos(string nombre)
    {
        Assert.True(ProveedorNombreValidator.EsValido(nombre));
    }

    [Theory]
    [InlineData("Empresa @ Central")]
    [InlineData("Proveedor #1")]
    [InlineData("Empresa <Central>")]
    public void EsValido_DebeRechazarCaracteresNoPermitidos(string nombre)
    {
        Assert.False(ProveedorNombreValidator.EsValido(nombre));
    }
}
