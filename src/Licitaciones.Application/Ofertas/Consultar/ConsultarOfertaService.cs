using Licitaciones.Domain.Common;

namespace Licitaciones.Application.Ofertas.Consultar;

public sealed class ConsultarOfertaService
{
    private readonly IOfertaConsultaRepository _repository;

    public ConsultarOfertaService(IOfertaConsultaRepository repository) =>
        _repository = repository;

    public async Task<PaginaOfertas> ListarAsync(
        ConsultarOfertasRequest consulta,
        CancellationToken cancellationToken = default)
    {
        ValidarConsulta(consulta);
        var ofertas = await _repository.ListarAsync(consulta.LicitacionId, cancellationToken);
        var convertidas = await ConvertirAsync(ofertas, consulta.Moneda, cancellationToken);

        IEnumerable<OfertaConsultaDto> filtradas = convertidas;
        if (!string.IsNullOrWhiteSpace(consulta.Proveedor))
        {
            filtradas = filtradas.Where(x => x.ProveedorNombre.Contains(
                consulta.Proveedor.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        filtradas = (consulta.OrdenarPor.ToLowerInvariant(), consulta.Descendente) switch
        {
            ("proveedor", false) => filtradas.OrderBy(x => x.ProveedorNombre),
            ("proveedor", true) => filtradas.OrderByDescending(x => x.ProveedorNombre),
            ("fecharegistro", false) => filtradas.OrderBy(x => x.FechaRegistro),
            ("fecharegistro", true) => filtradas.OrderByDescending(x => x.FechaRegistro),
            ("monto", true) => filtradas.OrderByDescending(x => x.Monto),
            _ => filtradas.OrderBy(x => x.Monto)
        };

        var todas = filtradas.ToArray();
        var items = todas.Skip((consulta.Pagina - 1) * consulta.TamanoPagina)
            .Take(consulta.TamanoPagina)
            .ToArray();
        return new PaginaOfertas(items, todas.Length, consulta.Pagina, consulta.TamanoPagina);
    }

    private static void ValidarConsulta(ConsultarOfertasRequest consulta)
    {
        if (consulta.LicitacionId == Guid.Empty)
        {
            throw new DomainException("La licitaciÃ³n es obligatoria.");
        }

        if (consulta.Pagina <= 0 || consulta.TamanoPagina is <= 0 or > 100)
        {
            throw new DomainException("La paginaciÃ³n solicitada no es vÃ¡lida.");
        }

        if (consulta.OrdenarPor.ToLowerInvariant() is not ("monto" or "proveedor" or "fecharegistro"))
        {
            throw new DomainException("El campo de ordenamiento no es vÃ¡lido.");
        }
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

        if (monedaNormalizada == "USD" && (tipoCambio is null || tipoCambio.Valor <= 0))
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
                monedaNormalizada == "USD" ? x.Monto / tipoCambio!.Valor : x.Monto,
                monedaNormalizada,
                x.FechaRegistro,
                x.Id == mejorOfertaId,
                monedaNormalizada == "USD" ? tipoCambio!.Valor : null,
                monedaNormalizada == "USD" ? tipoCambio!.Fecha : null))
            .ToArray();
    }
}

