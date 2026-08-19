using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.Application.Licitaciones.Crear;

public sealed class CrearLicitacionService
{
    private readonly ILicitacionRepository _repository;

    public CrearLicitacionService(ILicitacionRepository repository)
    {
        _repository = repository;
    }

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
