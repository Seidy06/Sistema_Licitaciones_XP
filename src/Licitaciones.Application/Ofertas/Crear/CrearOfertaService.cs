using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;
using Licitaciones.Domain.Ofertas;

namespace Licitaciones.Application.Ofertas.Crear;

public sealed class CrearOfertaService
{
    public const string ErrorDuplicada =
        "El proveedor ya tiene una oferta activa para esta licitacion.";

    private readonly IOfertaRepository _repository;
    private readonly IClock _clock;

    public CrearOfertaService(IOfertaRepository repository, IClock clock)
    {
        _repository = repository;
        _clock = clock;
    }

    public async Task<OfertaDto> CrearAsync(
        CrearOfertaRequest request,
        CancellationToken cancellationToken = default)
    {
        var licitacion = await _repository.ObtenerLicitacionPorIdAsync(
            request.LicitacionId, cancellationToken)
            ?? throw new DomainException("La licitacion indicada no existe.");

        if (licitacion.Estado != EstadoLicitacion.Publicada)
        {
            throw new DomainException(
                "La licitacion debe estar publicada para registrar ofertas.");
        }

        if (licitacion.EstaVencida(_clock))
        {
            throw new DomainException(
                "La licitacion esta vencida y no admite nuevas ofertas.");
        }

        if (await _repository.ExisteOfertaAsync(
            request.LicitacionId, request.ProveedorId, cancellationToken))
        {
            throw new DomainException(ErrorDuplicada);
        }

        if (request.Monto > licitacion.Presupuesto)
        {
            throw new DomainException(
                "El monto de la oferta no puede superar el presupuesto de la licitacion.");
        }

        if (request.Monto <= 0)
        {
            throw new DomainException("El monto de la oferta debe ser mayor que cero.");
        }

        _ = await _repository.ObtenerProveedorPorIdAsync(
            request.ProveedorId, cancellationToken)
            ?? throw new DomainException("El proveedor indicado no existe.");

        var oferta = Oferta.Crear(
            request.LicitacionId, request.ProveedorId, request.Monto, _clock);

        await _repository.AgregarAsync(oferta, cancellationToken);
        await _repository.GuardarCambiosAsync(cancellationToken);

        return new OfertaDto(
            oferta.Id,
            oferta.LicitacionId,
            oferta.ProveedorId,
            oferta.Monto,
            oferta.FechaRegistro);
    }
}
