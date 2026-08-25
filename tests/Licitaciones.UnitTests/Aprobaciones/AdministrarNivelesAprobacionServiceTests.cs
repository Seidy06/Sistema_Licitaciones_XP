using Licitaciones.Application.Aprobaciones;
using Licitaciones.Domain.Aprobaciones;
using Licitaciones.Domain.Common;

namespace Licitaciones.UnitTests.Aprobaciones;

public sealed class AdministrarNivelesAprobacionServiceTests
{
    [Fact]
    [Trait("HU", "HU-28")]
    public async Task CrearAsync_ConTraslapeActivo_DebeRechazarConConflicto()
    {
        var repositorio = new RepositorioFalso(existeTraslape: true);
        var service = new AdministrarNivelesAprobacionService(repositorio);

        await Assert.ThrowsAsync<NivelAprobacionConflictoException>(
            () => service.CrearAsync("Operativo", 0m, 5_000_000m));
        Assert.Empty(repositorio.Agregados);
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task CrearAsync_SinTraslape_DebeAgregarNivelActivoYRetornarDto()
    {
        var repositorio = new RepositorioFalso(existeTraslape: false);
        var service = new AdministrarNivelesAprobacionService(repositorio);

        var dto = await service.CrearAsync("Operativo", 0m, 5_000_000m);

        var agregado = Assert.Single(repositorio.Agregados);
        Assert.Equal(dto.Id, agregado.Id);
        Assert.Equal("Operativo", dto.Nombre);
        Assert.True(agregado.Activo);
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task DesactivarAsync_NivelActivoExistente_DebeDesactivarloYRetornarTrue()
    {
        var nivel = NivelAprobacion.Crear("Operativo", 0m, 5_000_000m);
        var repositorio = new RepositorioFalso(existeTraslape: false, porId: nivel);
        var service = new AdministrarNivelesAprobacionService(repositorio);

        var desactivado = await service.DesactivarAsync(nivel.Id);

        Assert.True(desactivado);
        Assert.False(nivel.Activo);
        Assert.True(repositorio.GuardoCambios);
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task DesactivarAsync_NivelInexistente_DebeRetornarFalse()
    {
        var repositorio = new RepositorioFalso(existeTraslape: false);
        var service = new AdministrarNivelesAprobacionService(repositorio);

        var desactivado = await service.DesactivarAsync(id: 99);

        Assert.False(desactivado);
        Assert.False(repositorio.GuardoCambios);
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task ListarAsync_ConFiltroNombre_DebeAplicarCoincidenciaParcial()
    {
        var repositorio = new RepositorioFalso(existeTraslape: false);
        await repositorio.PrepararAsync(
            NivelAprobacion.Crear("Operativo", 0m, 5_000_000m),
            NivelAprobacion.Crear("Gerencial", 5_000_000m, 10_000_000m),
            NivelAprobacion.Crear("Directivo", 10_000_000m, null));
        var service = new AdministrarNivelesAprobacionService(repositorio);

        var pagina = await service.ListarAsync(
            new NivelesAprobacionConsultaRequest(Nombre: "ger"));

        var item = Assert.Single(pagina.Items);
        Assert.Equal("Gerencial", item.Nombre);
        Assert.Equal(1, pagina.Total);
    }

    [Fact]
    [Trait("HU", "HU-28")]
    public async Task ListarAsync_ConCampoOrdenInvalido_DebeRechazarlo()
    {
        var service = new AdministrarNivelesAprobacionService(
            new RepositorioFalso(existeTraslape: false));

        await Assert.ThrowsAsync<DomainException>(() => service.ListarAsync(
            new NivelesAprobacionConsultaRequest(OrdenarPor: "montoMaximo")));
    }

    private sealed class RepositorioFalso : INivelAprobacionRepository
    {
        private readonly bool _existeTraslape;
        private readonly NivelAprobacion? _porId;

        public RepositorioFalso(bool existeTraslape, NivelAprobacion? porId = null)
        {
            _existeTraslape = existeTraslape;
            _porId = porId;
        }

        public List<NivelAprobacion> Agregados { get; } = [];

        public bool GuardoCambios { get; private set; }

        private IReadOnlyList<NivelAprobacion> Niveles { get; set; } = [];

        public Task PrepararAsync(params NivelAprobacion[] niveles)
        {
            Niveles = niveles;
            return Task.CompletedTask;
        }

        public Task<bool> ExisteTraslapeActivoAsync(
            decimal montoMinimo,
            decimal? montoMaximo,
            int? excludeId = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_existeTraslape);

        public Task AgregarAsync(
            NivelAprobacion nivel,
            CancellationToken cancellationToken = default)
        {
            Agregados.Add(nivel);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<NivelAprobacion>> ListarActivosAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Niveles);

        public Task<NivelAprobacion?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_porId is not null && _porId.Id == id ? _porId : null);

        public Task GuardarCambiosAsync(CancellationToken cancellationToken = default)
        {
            GuardoCambios = true;
            return Task.CompletedTask;
        }
    }
}
