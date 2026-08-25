using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;

namespace Licitaciones.Application.Ofertas.Editar;

public sealed class EditarOfertaService
{
    private readonly IEditarOfertaRepository _repository;
    private readonly IClock _clock;

    public EditarOfertaService(IEditarOfertaRepository repository, IClock clock)
    {
        _repository = repository;
        _clock = clock;
    }

    public async Task<OfertaDto> EditarAsync(
        EditarOfertaRequest request,
        CancellationToken cancellationToken = default)
    {
        var oferta = await _repository.ObtenerPorIdAsync(request.Id, cancellationToken)
            ?? throw new DomainException("La oferta indicada no existe.");

        var licitacion = await _repository.ObtenerLicitacionPorIdAsync(
            oferta.LicitacionId, cancellationToken)
            ?? throw new DomainException("La licitación asociada no existe.");

        if (licitacion.Estado != EstadoLicitacion.Publicada)
        {
            throw new DomainException(
                $"No se puede editar una oferta de una licitación en estado {licitacion.Estado}.",
                OfertaErrorCodes.NoProcesable);
        }

        if (licitacion.EstaVencida(_clock))
        {
            throw new DomainException(
                "No se puede editar una oferta de una licitación vencida.",
                OfertaErrorCodes.NoProcesable);
        }

        if (request.Monto > licitacion.Presupuesto)
        {
            throw new DomainException(
                "El monto de la oferta no puede superar el presupuesto de la licitación.",
                OfertaErrorCodes.NoProcesable);
        }

        oferta.Editar(request.Monto, _clock);
        await _repository.GuardarCambiosAsync(cancellationToken);

        return OfertaDto.FromEntity(oferta);
    }
}
