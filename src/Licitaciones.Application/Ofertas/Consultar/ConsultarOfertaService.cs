using Licitaciones.Domain.Common;

namespace Licitaciones.Application.Ofertas.Consultar;

public sealed class ConsultarOfertaService
{
    private readonly IOfertaConsultaRepository _repository;

    public ConsultarOfertaService(IOfertaConsultaRepository repository) =>
        _repository = repository;

    public async Task<IReadOnlyList<OfertaConsultaDto>> ListarAsync(
        Guid licitacionId,
        string moneda,
        CancellationToken cancellationToken = default)
    {
        var ofertas = await _repository.ListarAsync(licitacionId, cancellationToken);
        return await ConvertirAsync(ofertas, moneda, cancellationToken);
    }

    public async Task<OfertaConsultaDto?> ObtenerAsync(
        Guid id,
        string moneda,
        CancellationToken cancellationToken = default)
    {
        var oferta = await _repository.ObtenerPorIdAsync(id, cancellationToken);
        if (oferta is null)
        {
            return null;
        }

        var ofertas = await _repository.ListarAsync(
            oferta.LicitacionId, cancellationToken);
        var convertidas = await ConvertirAsync(ofertas, moneda, cancellationToken);

        return convertidas.Single(x => x.Id == id);
    }

    private async Task<IReadOnlyList<OfertaConsultaDto>> ConvertirAsync(
        IReadOnlyList<OfertaConsultaRegistro> ofertas,
        string moneda,
        CancellationToken cancellationToken)
    {
        var monedaNormalizada = moneda.Trim().ToUpperInvariant();
        if (monedaNormalizada is not ("CRC" or "USD"))
        {
            throw new DomainException("La moneda debe ser CRC o USD.");
        }

        var tipoCambio = monedaNormalizada == "USD"
            ? await _repository.ObtenerTipoCambioUsdCrcAsync(cancellationToken)
            : null;

        if (monedaNormalizada == "USD" && tipoCambio is null or <= 0)
        {
            throw new DomainException("No existe un tipo de cambio activo para USD.");
        }

        var mejorOfertaId = ofertas
            .OrderBy(x => x.Monto)
            .ThenBy(x => x.FechaRegistro)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefault();

        return ofertas
            .Select(x => new OfertaConsultaDto(
                x.Id,
                x.ProveedorNombre,
                monedaNormalizada == "USD" ? x.Monto / tipoCambio!.Value : x.Monto,
                monedaNormalizada,
                x.FechaRegistro,
                x.Id == mejorOfertaId))
            .ToArray();
    }
}

