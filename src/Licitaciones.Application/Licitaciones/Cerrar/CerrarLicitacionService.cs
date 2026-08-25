using Licitaciones.Application.Licitaciones.Editar;
using Licitaciones.Domain.Common;

namespace Licitaciones.Application.Licitaciones.Cerrar;

/// <summary>
/// Servicio para cerrar licitaciones cambiando su estado a Cerrada.
/// </summary>
public sealed class CerrarLicitacionService
{
    private readonly ILicitacionRepository _repository;
    private readonly IClock _clock;

    public CerrarLicitacionService(ILicitacionRepository repository, IClock clock)
    {
        _repository = repository;
        _clock = clock;
    }

    /// <summary>
    /// Cierra una licitación existente.
    /// </summary>
    /// <param name="id">Identificador de la licitación a cerrar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO con los datos de la licitación cerrada.</returns>
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
