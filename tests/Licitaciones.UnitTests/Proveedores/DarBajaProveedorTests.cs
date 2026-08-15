using Licitaciones.Application.Proveedores.Eliminar;
using Licitaciones.Domain.Proveedores;
using Licitaciones.UnitTests.Common;

namespace Licitaciones.UnitTests.Proveedores;

public sealed class DarBajaProveedorTests
{
    [Fact]
    [Trait("HU", "HU-08")]
    public void DarDeBaja_ProveedorActivo_DebeEstablecerDeletedAt()
    {
        var proveedor = Proveedor.Crear("Proveedor histórico");
        var instante = new DateTimeOffset(2026, 8, 14, 18, 30, 0, TimeSpan.Zero);

        proveedor.DarDeBaja(instante);

        Assert.Equal(instante, proveedor.DeletedAt);
        Assert.True(proveedor.EstaEliminado);
        Assert.Equal("Proveedor histórico", proveedor.Nombre);
        Assert.Equal("PROVEEDOR HISTÓRICO", proveedor.NombreNormalizado);
    }

    [Fact]
    [Trait("HU", "HU-08")]
    public async Task DarDeBajaAsync_DebeUsarElRelojYActualizarSinEliminarFisicamente()
    {
        var proveedor = Proveedor.Crear("Proveedor activo");
        var instante = new DateTimeOffset(2026, 8, 14, 19, 0, 0, TimeSpan.Zero);
        var repository = new RepositorioBajaEnMemoria(proveedor);
        var service = new DarBajaProveedorService(repository, new FixedClock(instante));

        await service.DarDeBajaAsync(proveedor.Id);

        Assert.Same(proveedor, repository.ProveedorActualizado);
        Assert.False(repository.SeSolicitoEliminacionFisica);
        Assert.Equal(instante, proveedor.DeletedAt);
    }

    [Fact]
    [Trait("HU", "HU-08")]
    public async Task DarDeBajaAsync_ProveedorInexistente_DebeLanzarExcepcionControlada()
    {
        var service = new DarBajaProveedorService(
            new RepositorioBajaEnMemoria(null),
            new FixedClock(DateTimeOffset.UtcNow));

        await Assert.ThrowsAsync<ProveedorNoEncontradoException>(() =>
            service.DarDeBajaAsync(Guid.NewGuid()));
    }

    private sealed class RepositorioBajaEnMemoria : IProveedorBajaRepository
    {
        private readonly Proveedor? _proveedor;

        public RepositorioBajaEnMemoria(Proveedor? proveedor) => _proveedor = proveedor;

        public Proveedor? ProveedorActualizado { get; private set; }
        public bool SeSolicitoEliminacionFisica { get; private set; }

        public Task<Proveedor?> ObtenerActivoParaDarDeBajaAsync(
            Guid id,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_proveedor?.Id == id ? _proveedor : null);

        public Task ActualizarBajaAsync(
            Proveedor proveedor,
            CancellationToken cancellationToken = default)
        {
            ProveedorActualizado = proveedor;
            return Task.CompletedTask;
        }

        public Task EliminarFisicamenteAsync(
            Proveedor proveedor,
            CancellationToken cancellationToken = default)
        {
            SeSolicitoEliminacionFisica = true;
            return Task.CompletedTask;
        }
    }
}
