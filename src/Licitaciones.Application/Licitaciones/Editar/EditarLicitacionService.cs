using Licitaciones.Domain.Common;
using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Licitaciones.Editar;

/// <summary>
/// Servicio para editar licitaciones existentes con validación de presupuesto.
/// </summary>
public sealed class EditarLicitacionService
{
    private readonly ILicitacionRepository _repository;
    private readonly IClock _clock;

    public EditarLicitacionService(
        ILicitacionRepository repository,
        IClock clock)
    {
        _repository = repository;
        _clock = clock;
    }

    /// <summary>
    /// Actualiza los datos de una licitación validando el presupuesto mínimo.
    /// </summary>
    /// <param name="request">Datos a actualizar de la licitación.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO con los datos actualizados de la licitación.</returns>
    public async Task<LicitacionDto> EditarAsync(
        EditarLicitacionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var licitacion = await _repository.ObtenerPorIdAsync(
            request.Id, cancellationToken)
            ?? throw new LicitacionNoEncontradaException(request.Id);

        var montoMinimoOferta = await _repository.ObtenerMontoMinimoOfertaAsync(
            licitacion.Id, cancellationToken);

        if (montoMinimoOferta.HasValue && request.Presupuesto < montoMinimoOferta.Value)
        {
            throw new DomainException(
                $"El presupuesto no puede ser menor que la oferta registrada de {montoMinimoOferta.Value:N2}.");
        }

        var codigo = request.Codigo ?? licitacion.Codigo;
        var titulo = request.Titulo ?? licitacion.Titulo;
        var presupuesto = request.Presupuesto ?? licitacion.Presupuesto;
        var fechaCierre = request.FechaCierre ?? licitacion.FechaCierre;

        licitacion.Editar(
            codigo,
            titulo,
            presupuesto,
            fechaCierre,
            _clock);

        await _repository.GuardarCambiosAsync(cancellationToken);

        return LicitacionDto.FromEntity(licitacion);
    }
}
