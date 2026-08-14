using Licitaciones.Application.Proveedores;
using Licitaciones.Application.Proveedores.Editar;
using Licitaciones.Domain.Proveedores;

namespace Licitaciones.UnitTests.Proveedores;

public sealed class EditarProveedorServiceTests
{
    [Fact]
    [Trait("HU", "HU-07")]
    public void Editar_DebeCambiarElNombreYReutilizarLaNormalizacionDeHu06()
    {
        var proveedor = Proveedor.Crear("Nombre anterior");

        proveedor.Editar("  Cafe\u0301    Central  ");

        Assert.Equal("Café Central", proveedor.Nombre);
        Assert.Equal(
            ProveedorNombreNormalizer.Normalizar("  Cafe\u0301    Central  "),
            proveedor.NombreNormalizado);
        Assert.Equal("CAFÉ CENTRAL", proveedor.NombreNormalizado);
    }

    [Fact]
    [Trait("HU", "HU-07")]
    public async Task EditarAsync_DebeRechazarNombreNormalizadoDeOtroProveedor()
    {
        var proveedor = Proveedor.Crear("Nombre anterior");
        var repository = new RepositorioEnMemoria(proveedor)
        {
            IdDelProveedorDuplicado = Guid.NewGuid()
        };
        var service = new EditarProveedorService(repository);

        await Assert.ThrowsAsync<ProveedorDuplicadoException>(() =>
            service.EditarAsync(
                proveedor.Id,
                new EditarProveedorRequest(" empresa   central ", proveedor.Version)));

        Assert.Equal("EMPRESA CENTRAL", repository.NombreConsultado);
        Assert.Equal(proveedor.Id, repository.IdExcluidoDeDuplicados);
        Assert.Null(repository.ProveedorActualizado);
    }

    [Fact]
    [Trait("HU", "HU-07")]
    public async Task EditarAsync_NoDebeConsiderarAlPropioProveedorComoDuplicado()
    {
        var proveedor = Proveedor.Crear("Empresa Central");
        var repository = new RepositorioEnMemoria(proveedor)
        {
            IdDelProveedorDuplicado = proveedor.Id
        };
        var service = new EditarProveedorService(repository);

        var resultado = await service.EditarAsync(
            proveedor.Id,
            new EditarProveedorRequest("  empresa   central  ", proveedor.Version));

        Assert.Equal("Empresa Central", resultado.Nombre);
        Assert.Same(proveedor, repository.ProveedorActualizado);
        Assert.Equal(proveedor.Id, repository.IdExcluidoDeDuplicados);
    }

    [Fact]
    [Trait("HU", "HU-07")]
    public async Task EditarAsync_ProveedorInexistente_DebeLanzarExcepcionControlada()
    {
        var service = new EditarProveedorService(new RepositorioEnMemoria(null));

        await Assert.ThrowsAsync<ProveedorNoEncontradoException>(() =>
            service.EditarAsync(
                Guid.NewGuid(),
                new EditarProveedorRequest("Empresa Central", 1)));
    }

    [Fact]
    [Trait("HU", "HU-07")]
    public async Task EditarAsync_VersionDesactualizada_DebePropagarConflictoControlado()
    {
        var proveedor = Proveedor.Crear("Nombre anterior");
        var repository = new RepositorioEnMemoria(proveedor)
        {
            ConflictoAlActualizar = true
        };
        var service = new EditarProveedorService(repository);

        await Assert.ThrowsAsync<ProveedorConcurrenciaException>(() =>
            service.EditarAsync(
                proveedor.Id,
                new EditarProveedorRequest("Nombre nuevo", proveedor.Version)));
    }

    private sealed class RepositorioEnMemoria : IProveedorRepository
    {
        private readonly Proveedor? _proveedor;

        public RepositorioEnMemoria(Proveedor? proveedor) => _proveedor = proveedor;

        public Guid? IdDelProveedorDuplicado { get; init; }

        public bool ConflictoAlActualizar { get; init; }

        public string? NombreConsultado { get; private set; }

        public Guid? IdExcluidoDeDuplicados { get; private set; }

        public Proveedor? ProveedorActualizado { get; private set; }

        public Task<Proveedor?> ObtenerParaEditarAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_proveedor?.Id == id ? _proveedor : null);
        }

        public Task<bool> ExisteNombreNormalizadoAsync(
            string nombreNormalizado,
            Guid excluirProveedorId,
            CancellationToken cancellationToken = default)
        {
            NombreConsultado = nombreNormalizado;
            IdExcluidoDeDuplicados = excluirProveedorId;
            return Task.FromResult(
                IdDelProveedorDuplicado is Guid id && id != excluirProveedorId);
        }

        public Task ActualizarAsync(
            Proveedor proveedor,
            uint versionEsperada,
            CancellationToken cancellationToken = default)
        {
            if (ConflictoAlActualizar)
            {
                throw new ProveedorConcurrenciaException(proveedor.Id);
            }

            ProveedorActualizado = proveedor;
            return Task.CompletedTask;
        }

        public Task<bool> ExisteNombreNormalizadoAsync(
            string nombreNormalizado,
            CancellationToken cancellationToken = default) =>
            ExisteNombreNormalizadoAsync(nombreNormalizado, Guid.Empty, cancellationToken);

        public Task AgregarAsync(
            Proveedor proveedor,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
