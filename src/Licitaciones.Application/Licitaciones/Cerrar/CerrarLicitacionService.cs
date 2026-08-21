using Licitaciones.Application.Licitaciones.Editar;
using Licitaciones.Domain.Common;

namespace Licitaciones.Application.Licitaciones.Cerrar;

public sealed class CerrarLicitacionService
{
    private readonly ILicitacionRepository _repository;
    private readonly IClock _clock;

    public CerrarLicitacionService(ILicitacionRepository repository, IClock clock)
    {
        _repository = repository;
        _clock = clock;
    }

    public async Task<LicitacionDto> CerrarAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var licitacion = await _repository.ObtenerPorIdAsync(id, cancellationToken)
            ?? throw new LicitacionNoEncontradaException(id);

        licitacion.Cerrar(_clock);
        await _repository.GuardarCambiosAsync(cancellationToken);
        return LicitacionDto.FromEntity(licitacion);
    }
}
