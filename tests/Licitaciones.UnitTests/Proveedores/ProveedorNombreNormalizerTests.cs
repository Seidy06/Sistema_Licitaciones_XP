using Licitaciones.Domain.Proveedores;
using Xunit;

namespace Licitaciones.UnitTests.Proveedores;

public class ProveedorNombreNormalizerTests
{
    [Theory]
    [InlineData("  Empresa Central  ", "EMPRESA CENTRAL")]
    [InlineData("Empresa    Central", "EMPRESA CENTRAL")]
    [InlineData("empresa central", "EMPRESA CENTRAL")]
    [InlineData("EMPRESA CENTRAL", "EMPRESA CENTRAL")]
    public void Normalizar_DebeGenerarValorComparable(
        string nombre,
        string esperado)
    {
        var resultado = ProveedorNombreNormalizer.Normalizar(nombre);

        Assert.Equal(esperado, resultado);
    }

    [Fact]
    public void Normalizar_DebeNormalizarUnicode()
    {
        var compuesto = "Café Central";
        var descompuesto = "Cafe\u0301 Central";

        var resultadoCompuesto =
            ProveedorNombreNormalizer.Normalizar(compuesto);

        var resultadoDescompuesto =
            ProveedorNombreNormalizer.Normalizar(descompuesto);

        Assert.Equal(resultadoCompuesto, resultadoDescompuesto);
    }
}
