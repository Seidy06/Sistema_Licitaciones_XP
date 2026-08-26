using Licitaciones.Application.Licitaciones.Consultar;

namespace Licitaciones.Application.Aprobaciones;

/// <summary>
/// Servicio para resolver el nivel de aprobación aplicable según un monto.
/// </summary>
public sealed class ResolverNivelAprobacionService
{
    private readonly ILicitacionConsultaRepository _repository;

    public ResolverNivelAprobacionService(ILicitacionConsultaRepository repository) =>
        _repository = repository;

    /// <summary>
    /// Resuelve el nivel de aprobación aplicable para un monto dado.
    /// </summary>
    /// <param name="monto">Monto para el cual se desea resolver el nivel.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO del nivel de aprobación o null si no aplica ninguno.</returns>
    public Task<LicitacionNivelAprobacionDto?> ResolverAsync(
        decimal monto,
        CancellationToken cancellationToken = default) =>
        _repository.ObtenerNivelAprobacionAsync(monto, cancellationToken);
}
