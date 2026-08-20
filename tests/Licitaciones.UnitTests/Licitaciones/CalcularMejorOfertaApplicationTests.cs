using System.Text.Encodings.Web;
using System.Text.Json;

using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.Licitaciones.Consultar;
using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;
using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Licitaciones;

public sealed class CalcularMejorOfertaApplicationTests
{
    private static readonly JsonSerializerOptions OpcionesJson = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private static readonly DateTimeOffset Ahora =
        new(2026, 8, 20, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    [Trait("HU", "HU-16")]
    public async Task VariasOfertasConEmpate_DebeSeleccionarMenorMontoRegistradoPrimero()
    {
        var licitacion = CrearLicitacion(10_000m);
        var ofertaMayor = CrearOferta(licitacion.Id, 9_500m, Ahora);
        var ofertaPrimero = CrearOferta(licitacion.Id, 9_000m, Ahora.AddMinutes(1));
        var ofertaDespues = CrearOferta(licitacion.Id, 9_000m, Ahora.AddMinutes(2));

        var detalle = await ConsultarAsync(
            licitacion,
            [ofertaMayor, ofertaPrimero, ofertaDespues]);
        var json = Serializar(detalle);
        var mejorOferta = json.GetProperty("mejorOferta");

        Assert.Equal(9_000m, mejorOferta.GetProperty("monto").GetDecimal());
        Assert.True(
            mejorOferta.TryGetProperty("id", out var ofertaId),
            "La mejor oferta debe identificar cuál oferta ganó el desempate.");
        Assert.Equal(ofertaPrimero.Id, ofertaId.GetGuid());
    }

    [Fact]
    [Trait("HU", "HU-16")]
    public async Task SinOfertasValidas_DebeMostrarMensajeEspecifico()
    {
        var detalle = await ConsultarAsync(CrearLicitacion(10_000m), []);

        var json = JsonSerializer.Serialize(detalle, OpcionesJson);

        Assert.Contains("Sin ofertas válidas", json, StringComparison.Ordinal);
    }

    [Fact]
    [Trait("HU", "HU-16")]
    public async Task AhorroExactamenteDiezPorCiento_DebeClasificarOfertaConveniente()
    {
        var json = await ConsultarComoJsonAsync(10_000m, 9_000m);

        Assert.Contains("Oferta conveniente", json, StringComparison.Ordinal);
    }

    [Fact]
    [Trait("HU", "HU-16")]
    public async Task AhorroMayorACeroYMenorADiezPorCiento_DebeClasificarOfertaAceptable()
    {
        var json = await ConsultarComoJsonAsync(10_000m, 9_500m);

        Assert.Contains("Oferta aceptable", json, StringComparison.Ordinal);
    }

    [Fact]
    [Trait("HU", "HU-16")]
    public async Task OfertaIgualAlPresupuesto_DebeClasificarValidaSinAhorro()
    {
        var json = await ConsultarComoJsonAsync(10_000m, 10_000m);

        Assert.Contains("Oferta válida sin ahorro", json, StringComparison.Ordinal);
    }

    private static async Task<string> ConsultarComoJsonAsync(
        decimal presupuesto,
        decimal montoOferta)
    {
        var licitacion = CrearLicitacion(presupuesto);
        var detalle = await ConsultarAsync(
            licitacion,
            [CrearOferta(licitacion.Id, montoOferta, Ahora)]);

        return JsonSerializer.Serialize(detalle, OpcionesJson);
    }

    private static async Task<LicitacionDetalleDto> ConsultarAsync(
        Licitacion licitacion,
        IReadOnlyList<Oferta> ofertas)
    {
        var service = new ConsultarLicitacionService(
            new RepositorioConsulta(licitacion, ofertas));

        var detalle = await service.ObtenerDetalleAsync(
            licitacion.Id,
            new FixedClock(Ahora));

        return Assert.IsType<LicitacionDetalleDto>(detalle);
    }

    private static JsonElement Serializar(LicitacionDetalleDto detalle) =>
        JsonSerializer.SerializeToElement(
            detalle,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

    private static Licitacion CrearLicitacion(decimal presupuesto) =>
        Licitacion.Crear(
            $"HU16-{Guid.NewGuid():N}",
            "Compra para calcular mejor oferta",
            presupuesto,
            Ahora.AddDays(5));

    private static Oferta CrearOferta(
        Guid licitacionId,
        decimal monto,
        DateTimeOffset fechaRegistro) =>
        Oferta.Crear(
            licitacionId,
            Guid.NewGuid(),
            monto,
            new FixedClock(fechaRegistro));

    private sealed class RepositorioConsulta : ILicitacionConsultaRepository
    {
        private readonly Licitacion _licitacion;
        private readonly IReadOnlyList<Oferta> _ofertas;

        public RepositorioConsulta(
            Licitacion licitacion,
            IReadOnlyList<Oferta> ofertas)
        {
            _licitacion = licitacion;
            _ofertas = ofertas;
        }

        public Task<IReadOnlyList<Licitacion>> ListarAsync(
            ConsultarLicitacionesRequest consulta,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Licitacion>>([_licitacion]);

        public Task<Licitacion?> ObtenerPorIdAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<Licitacion?>(id == _licitacion.Id ? _licitacion : null);

        public Task<IReadOnlyList<Oferta>> ObtenerOfertasAsync(
            Guid licitacionId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Oferta>>(
                _ofertas
                    .Where(oferta => oferta.LicitacionId == licitacionId)
                    .ToArray());

        public Task<LicitacionNivelAprobacionDto?> ObtenerNivelAprobacionAsync(
            decimal montoOferta,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<LicitacionNivelAprobacionDto?>(null);
    }
}
