using Licitaciones.Domain.Common;

namespace Licitaciones.Application.Licitaciones.Eliminar;

/// <summary>
/// Servicio para dar de baja licitaciones activas.
/// </summary>
public sealed class EliminarLicitacionService
{
    private readonly ILicitacionBajaRepository _repository;
    private readonly IClock _clock;

    public EliminarLicitacionService(ILicitacionBajaRepository repository, IClock clock)
    {
        _repository = repository;
        _clock = clock;
    }

    /// <summary>
    /// Da de baja una licitación activa.
    /// </summary>
    /// <param name="id">Identificador de la licitación a dar de baja.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    public async Task DarDeBajaAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var licitacion = await _repository.ObtenerActivaParaDarDeBajaAsync(
            id, cancellationToken);

        if (licitacion is null)
        {
            throw new LicitacionNoEncontradaParaBajaException(id);
        }

        licitacion.DarDeBaja(_clock.UtcNow());
        await _repository.ActualizarBajaAsync(licitacion, cancellationToken);
    }
}
