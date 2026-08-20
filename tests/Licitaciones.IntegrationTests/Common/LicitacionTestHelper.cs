using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.IntegrationTests.Common;

internal static class LicitacionTestHelper
{
    internal static Licitacion PublicarLicitacion(
        string codigo, DateTimeOffset fechaCierre)
    {
        var licitacion = Licitacion.Crear(
            codigo,
            "Compra para pruebas HU-13",
            10_000m,
            fechaCierre);

        licitacion.Publicar(new FixedClock(fechaCierre.AddDays(-5)));
        return licitacion;
    }

    internal sealed class FixedClock : IClock
    {
        private readonly DateTimeOffset _value;
        public FixedClock(DateTimeOffset value) => _value = value;
        public DateTimeOffset UtcNow() => _value;
    }
}
