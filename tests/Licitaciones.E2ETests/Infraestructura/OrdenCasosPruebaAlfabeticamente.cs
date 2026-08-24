using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestCaseOrderer(
    "Licitaciones.E2ETests.Infraestructura.OrdenCasosPruebaAlfabeticamente",
    "Licitaciones.E2ETests")]
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Licitaciones.E2ETests.Infraestructura;

public sealed class OrdenCasosPruebaAlfabeticamente : ITestCaseOrderer
{
    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase
    {
        var casos = new List<TTestCase>(testCases);

        casos.Sort((izquierdo, derecho) =>
        {
            var clase = string.CompareOrdinal(
                izquierdo.TestMethod.TestClass.Class.Name,
                derecho.TestMethod.TestClass.Class.Name);
            if (clase != 0)
            {
                return clase;
            }

            return string.CompareOrdinal(
                izquierdo.TestMethod.Method.Name,
                derecho.TestMethod.Method.Name);
        });

        return casos;
    }
}
