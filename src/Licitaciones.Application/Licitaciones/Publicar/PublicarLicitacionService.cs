using Licitaciones.Application.Licitaciones.Editar;
using Licitaciones.Domain.Common;

namespace Licitaciones.Application.Licitaciones.Publicar;

/// <summary>
/// Servicio para publicar licitaciones cambiando su estado a Publicada.
/// </summary>
public sealed class PublicarLicitacionService
{
    private readonly ILicitacionRepository _repository;
    private readonly IClock _clock;

    public PublicarLicitacionService(ILicitacionRepository repository, IClock clock)
    {
        _repository = repository;
        _clock = clock;
    }

    /// <summary>
    /// Publica una licitación existente.
    /// </summary>
    /// <param name="id">Identificador de la licitación a publicar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO con los datos de la licitación publicada.</returns>
    public async Task<LicitacionDto> PublicarAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var licitacion = await _repository.ObtenerPorIdAsync(id, cancellationToken)
            ?? throw new LicitacionNoEncontradaException(id);

        licitacion.Publicar(_clock);
        await _repository.GuardarCambiosAsync(cancellationToken);
        return LicitacionDto.FromEntity(licitacion);
    }
}
