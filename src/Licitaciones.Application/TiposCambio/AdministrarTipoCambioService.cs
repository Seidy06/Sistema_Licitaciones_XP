using Licitaciones.Domain.TiposCambio;

namespace Licitaciones.Application.TiposCambio;

public sealed class AdministrarTipoCambioService
{
    private readonly ITipoCambioRepository _repository;

    public AdministrarTipoCambioService(ITipoCambioRepository repository) =>
        _repository = repository;

    public async Task<TipoCambioDto> GuardarAsync(
        decimal valor,
        DateOnly fecha,
        CancellationToken cancellationToken = default)
    {
        var tipoCambio = TipoCambio.Crear(valor, fecha);
        await _repository.ReemplazarActivoAsync(tipoCambio, cancellationToken);
        return Mapear(tipoCambio);
    }

    public async Task<TipoCambioDto?> ObtenerActivoAsync(
        CancellationToken cancellationToken = default)
    {
        var tipoCambio = await _repository.ObtenerActivoAsync(cancellationToken);
        return tipoCambio is null ? null : Mapear(tipoCambio);
    }

    private static TipoCambioDto Mapear(TipoCambio tipoCambio) => new(
        tipoCambio.Id,
        tipoCambio.MonedaOrigen,
        tipoCambio.MonedaDestino,
        tipoCambio.Valor,
        tipoCambio.Fecha,
        tipoCambio.Activo);
}
