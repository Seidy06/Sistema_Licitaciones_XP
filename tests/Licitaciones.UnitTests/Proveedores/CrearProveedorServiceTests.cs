using Licitaciones.Application.Proveedores;
using Licitaciones.Application.Proveedores.Crear;
using Licitaciones.Domain.Proveedores;

namespace Licitaciones.UnitTests.Proveedores;

public class CrearProveedorServiceTests
{
    [Fact]
    public async Task CrearAsync_DebeAgregarProveedorYRetornarDto()
    {
        var repository = new RepositorioEnMemoria();
        var service = new CrearProveedorService(repository);

        var resultado = await service.CrearAsync(
            new CrearProveedorRequest("  Empresa   Central  "));

        Assert.Equal("Empresa Central", resultado.Nombre);
        Assert.Equal("EMPRESA CENTRAL", resultado.NombreNormalizado);
        Assert.NotEqual(Guid.Empty, resultado.Id);
        Assert.NotNull(repository.ProveedorAgregado);
        Assert.Equal(resultado.Id, repository.ProveedorAgregado!.Id);
    }

    [Fact]
    public async Task CrearAsync_DebeRechazarNombreDuplicado()
    {
        var repository = new RepositorioEnMemoria { Existe = true };
        var service = new CrearProveedorService(repository);

        await Assert.ThrowsAsync<ProveedorDuplicadoException>(() =>
            service.CrearAsync(new CrearProveedorRequest("empresa central")));

        Assert.Equal("EMPRESA CENTRAL", repository.NombreConsultado);
        Assert.Null(repository.ProveedorAgregado);
    }

    private sealed class RepositorioEnMemoria : IProveedorRepository
    {
        public bool Existe { get; init; }

        public string? NombreConsultado { get; private set; }

        public Proveedor? ProveedorAgregado { get; private set; }

        public Task<bool> ExisteNombreNormalizadoAsync(
            string nombreNormalizado,
            CancellationToken cancellationToken = default)
        {
            NombreConsultado = nombreNormalizado;
            return Task.FromResult(Existe);
        }

        public Task AgregarAsync(
            Proveedor proveedor,
            CancellationToken cancellationToken = default)
        {
            ProveedorAgregado = proveedor;
            return Task.CompletedTask;
        }
    }
}
