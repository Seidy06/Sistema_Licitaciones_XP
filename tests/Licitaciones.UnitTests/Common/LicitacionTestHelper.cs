using System.Reflection;

using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.UnitTests.Common;

internal static class LicitacionTestHelper
{
    internal static void EstablecerEstado(
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
