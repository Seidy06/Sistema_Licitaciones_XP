using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;

namespace Licitaciones.Application.Ofertas.Eliminar;

public sealed class EliminarOfertaService
{
    private readonly IEliminarOfertaRepository _repository;
    private readonly IClock _clock;

    public EliminarOfertaService(IEliminarOfertaRepository repository, IClock clock)
    {
        _repository = repository;
        _clock = clock;
    }

    public async Task<bool> EliminarAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        var oferta = await _repository.ObtenerPorIdAsync(id, cancellationToken);
        if (oferta is null) return false;

        var licitacion = await _repository.ObtenerLicitacionPorIdAsync(
            oferta.LicitacionId, cancellationToken)
            ?? throw new DomainException("La licitación asociada no existe.");

        if (licitacion.Estado != EstadoLicitacion.Publicada)
        {
            throw new DomainException(
                $"No se puede eliminar una oferta de una licitación en estado {licitacion.Estado}.",
                OfertaErrorCodes.NoProcesable);
        }

        if (licitacion.EstaVencida(_clock))
        {
            throw new DomainException(
                "No se puede eliminar una oferta de una licitación vencida.",
                OfertaErrorCodes.NoProcesable);
        }

        oferta.Eliminar(_clock.UtcNow());
        await _repository.GuardarCambiosAsync(cancellationToken);
        return true;
    }
}
