using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Licitaciones.Crear;

/// <summary>
/// Servicio para crear nuevas licitaciones con validación de código duplicado.
/// </summary>
public sealed class CrearLicitacionService
{
    private readonly ILicitacionRepository _repository;

    public CrearLicitacionService(ILicitacionRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Registra una licitación nueva validando que el código no esté duplicado.
    /// </summary>
    /// <param name="request">Datos de la licitación a crear.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO con los datos de la licitación creada.</returns>
    public async Task<LicitacionDto> CrearAsync(
        CrearLicitacionRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var licitacion = Licitacion.Crear(
            request.Codigo,
            request.Titulo,
            request.Presupuesto,
            request.FechaCierre);

        if (await _repository.ExisteCodigoNormalizadoAsync(
                licitacion.CodigoNormalizado,
                cancellationToken))
        {
            throw new LicitacionDuplicadoException(request.Codigo);
        }

        await _repository.AgregarAsync(licitacion, cancellationToken);

        return LicitacionDto.FromEntity(licitacion);
    }
}
