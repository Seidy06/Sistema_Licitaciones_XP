using Licitaciones.Application.Licitaciones.Consultar;

namespace Licitaciones.Application.Aprobaciones;

public sealed class ResolverNivelAprobacionService
{
    private readonly ILicitacionConsultaRepository _repository;

    public ResolverNivelAprobacionService(ILicitacionConsultaRepository repository) =>
        _repository = repository;

    public Task<LicitacionNivelAprobacionDto?> ResolverNivelAprobacion(
        decimal monto,
        CancellationToken cancellationToken = default) =>
        _repository.ObtenerNivelAprobacionAsync(monto, cancellationToken);
}
