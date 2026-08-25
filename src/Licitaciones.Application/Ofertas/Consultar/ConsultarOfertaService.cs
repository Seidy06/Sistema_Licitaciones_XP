using Licitaciones.Application.TiposCambio;
using Licitaciones.Domain.Common;
using Licitaciones.Domain.TiposCambio;

namespace Licitaciones.Application.Ofertas.Consultar;

/// <summary>
/// Servicio para consultar ofertas con conversión de moneda y paginación.
/// </summary>
public sealed class ConsultarOfertaService
{
    private readonly IOfertaConsultaRepository _repository;
    private readonly ITipoCambioRepository _tiposCambio;

    public ConsultarOfertaService(
        IOfertaConsultaRepository repository,
        ITipoCambioRepository tiposCambio)
    {
        _repository = repository;
        _tiposCambio = tiposCambio;
    }

    /// <summary>
    /// Lista ofertas de una licitación con conversión de moneda y filtrado.
    /// </summary>
    /// <param name="consulta">Parámetros de filtrado, moneda y paginación.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Página de resultados con las ofertas encontradas.</returns>
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

    /// <summary>
    /// Obtiene una oferta por su identificador con conversión de moneda.
    /// </summary>
    /// <param name="id">Identificador de la oferta.</param>
    /// <param name="moneda">Moneda destino para la conversión (CRC o USD).</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO de la oferta o null si no existe.</returns>
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

    /// <summary>
    /// Lista ofertas de un proveedor específico con conversión de moneda y paginación.
    /// </summary>
    /// <param name="proveedorId">Identificador del proveedor.</param>
    /// <param name="moneda">Moneda destino para la conversión (CRC o USD).</param>
    /// <param name="licitacionCodigo">Filtro opcional por código de licitación.</param>
    /// <param name="ordenarPor">Campo de ordenamiento (monto, licitacion, fecharegistro).</param>
    /// <param name="descendente">Indica si el orden es descendente.</param>
    /// <param name="pagina">Número de página.</param>
    /// <param name="tamanoPagina">Tamaño de la página.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Página de resultados con las ofertas del proveedor.</returns>
    public async Task<PaginaOfertas> ListarPorProveedorAsync(
        Guid proveedorId,
        string moneda,
        string? licitacionCodigo,
        string ordenarPor,
        bool descendente,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken = default)
    {
        if (pagina <= 0 || tamanoPagina is <= 0 or > 100)
        {
            throw new DomainException("La paginación solicitada no es válida.");
        }

        if (ordenarPor.ToLowerInvariant() is not ("monto" or "licitacion" or "fecharegistro"))
        {
            throw new DomainException("El campo de ordenamiento no es válido.");
        }

        var ofertas = await _repository.ListarPorProveedorIdAsync(
            proveedorId, cancellationToken);
        var convertidas = await ConvertirAsync(ofertas, moneda, cancellationToken);

        IEnumerable<OfertaConsultaDto> filtradas = convertidas;
        if (!string.IsNullOrWhiteSpace(licitacionCodigo))
        {
            filtradas = filtradas.Where(x => x.LicitacionId.ToString().Contains(
                licitacionCodigo.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        filtradas = (ordenarPor.ToLowerInvariant(), descendente) switch
        {
            ("licitacion", false) => filtradas.OrderBy(x => x.LicitacionId),
            ("licitacion", true) => filtradas.OrderByDescending(x => x.LicitacionId),
            ("fecharegistro", false) => filtradas.OrderBy(x => x.FechaRegistro),
            ("fecharegistro", true) => filtradas.OrderByDescending(x => x.FechaRegistro),
            ("monto", true) => filtradas.OrderByDescending(x => x.Monto),
            _ => filtradas.OrderBy(x => x.Monto)
        };

        var todas = filtradas.ToArray();
        var items = todas.Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToArray();
        return new PaginaOfertas(items, todas.Length, pagina, tamanoPagina);
    }

    private async Task<IReadOnlyList<OfertaConsultaDto>> ConvertirAsync(
        IReadOnlyList<OfertaConsultaRegistro> ofertas,
        string moneda,
        CancellationToken cancellationToken)
    {
        var monedaNormalizada = moneda.Trim().ToUpperInvariant();
        var esDolares = monedaNormalizada == TipoCambio.MonedaOrigenPredeterminada;
        if (!esDolares && monedaNormalizada != TipoCambio.MonedaDestinoPredeterminada)
        {
            throw new DomainException("La moneda debe ser CRC o USD.");
        }

        var tipoCambio = esDolares
            ? await _tiposCambio.ObtenerActivoAsync(cancellationToken)
            : null;

        if (esDolares && (tipoCambio is null || tipoCambio.Valor <= 0))
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
                x.LicitacionId,
                x.ProveedorNombre,
                esDolares ? x.Monto / tipoCambio!.Valor : x.Monto,
                monedaNormalizada,
                x.FechaRegistro,
                x.Id == mejorOfertaId,
                esDolares ? tipoCambio!.Valor : null,
                esDolares ? tipoCambio!.Fecha : null))
            .ToArray();
    }
}

