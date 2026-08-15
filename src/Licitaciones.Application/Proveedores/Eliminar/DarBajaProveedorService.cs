using Licitaciones.Domain.Common;

namespace Licitaciones.Application.Proveedores.Eliminar;

public sealed class DarBajaProveedorService
{
    private readonly IProveedorBajaRepository _repository;
    private readonly IClock _clock;

    public DarBajaProveedorService(IProveedorBajaRepository repository, IClock clock)
    {
        _repository = repository;
        _clock = clock;
    }

    public async Task DarDeBajaAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var proveedor = await _repository.ObtenerActivoParaDarDeBajaAsync(
            id, cancellationToken);

        if (proveedor is null)
        {
            throw new ProveedorNoEncontradoException(id);
        }

        proveedor.DarDeBaja(_clock.UtcNow());
        await _repository.ActualizarBajaAsync(proveedor, cancellationToken);
    }
}
