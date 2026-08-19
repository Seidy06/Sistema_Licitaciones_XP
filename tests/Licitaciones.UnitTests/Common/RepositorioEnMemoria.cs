using Licitaciones.Application.Licitaciones;
using Licitaciones.Domain.Licitaciones;

namespace Licitaciones.UnitTests.Common;

internal sealed class RepositorioEnMemoria : ILicitacionRepository
{
    private readonly Dictionary<Guid, Licitacion> _licitaciones = new();

    public RepositorioEnMemoria()
    {
    }

    public RepositorioEnMemoria(Licitacion licitacion)
    {
        _licitaciones[licitacion.Id] = licitacion;
    }

    public bool CodigoNormalizadoExiste { get; init; }

    public string? CodigoConsultado { get; private set; }

    public Licitacion? LicitacionAgregada { get; private set; }

    public decimal? MontoMinimoOferta { get; init; }

    public Task<bool> ExisteCodigoNormalizadoAsync(
        string codigoNormalizado,
        CancellationToken cancellationToken = default)
    {
        CodigoConsultado = codigoNormalizado;
        return Task.FromResult(CodigoNormalizadoExiste);
    }

    public Task AgregarAsync(
        Licitacion licitacion,
        CancellationToken cancellationToken = default)
    {
        LicitacionAgregada = licitacion;
        return Task.CompletedTask;
    }

    public Task<Licitacion?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _licitaciones.TryGetValue(id, out var licitacion);
        return Task.FromResult(licitacion);
    }

    public Task<decimal?> ObtenerMontoMinimoOfertaAsync(
        Guid licitacionId,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(MontoMinimoOferta);
    }

    public Task GuardarCambiosAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
