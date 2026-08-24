using Licitaciones.Application.TiposCambio;
using Licitaciones.Domain.TiposCambio;

namespace Licitaciones.UnitTests.Common;

internal sealed class RepositorioTipoCambioEnMemoria : ITipoCambioRepository
{
    private IReadOnlyList<TipoCambio> _tipos;

    public RepositorioTipoCambioEnMemoria(params TipoCambio[] tipos) =>
        _tipos = tipos;

    public List<TipoCambio> Reemplazados { get; } = [];

    public Task PrepararAsync(params TipoCambio[] tipos)
    {
        _tipos = tipos;
        return Task.CompletedTask;
    }

    public Task<TipoCambio?> ObtenerActivoAsync(
        CancellationToken cancellationToken = default) =>
        Task.FromResult(_tipos.FirstOrDefault(tipo => tipo.Activo));

    public Task ReemplazarActivoAsync(
        TipoCambio tipoCambio,
        CancellationToken cancellationToken = default)
    {
        Reemplazados.Add(tipoCambio);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TipoCambio>> ListarTodosAsync(
        CancellationToken cancellationToken = default) =>
        Task.FromResult(_tipos);
}
