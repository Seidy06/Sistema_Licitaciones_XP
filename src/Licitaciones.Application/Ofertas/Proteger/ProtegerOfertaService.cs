using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Ofertas.Proteger;

public sealed class ProtegerOfertaService
{
    private readonly IProteccionOfertaRepository _repository;
    private readonly IClock _clock;

    public ProtegerOfertaService(
        IProteccionOfertaRepository repository,
        IClock clock)
    {
        _repository = repository;
        _clock = clock;
    }

    public async Task RechazarCambioAsync(
        Guid ofertaId,
        CancellationToken cancellationToken = default)
    {
        var licitacion = await _repository.ObtenerLicitacionPorOfertaIdAsync(
            ofertaId, cancellationToken);

        if (licitacion?.EstadoEfectivo(_clock) == EstadoLicitacion.Cerrada)
        {
            throw new DomainException(
                "No se puede editar ni eliminar una oferta de una licitacion cerrada.",
                OfertaErrorCodes.NoProcesable);
        }

        throw new DomainException(
            "Las ofertas registradas no se pueden editar ni eliminar.",
            OfertaErrorCodes.NoProcesable);
    }
}
