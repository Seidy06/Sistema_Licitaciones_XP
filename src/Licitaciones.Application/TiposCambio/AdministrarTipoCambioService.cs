using Licitaciones.Application.Common;
using Licitaciones.Domain.Common;
using Licitaciones.Domain.TiposCambio;

namespace Licitaciones.Application.TiposCambio;

/// <summary>
/// Servicio para administrar tipos de cambio: crear, consultar, actualizar y activar.
/// </summary>
public sealed class AdministrarTipoCambioService
{
    private readonly ITipoCambioRepository _repository;

    public AdministrarTipoCambioService(ITipoCambioRepository repository) =>
        _repository = repository;

    /// <summary>
    /// Guarda un nuevo tipo de cambio reemplazando el activo actual.
    /// </summary>
    /// <param name="valor">Valor del tipo de cambio.</param>
    /// <param name="fecha">Fecha del tipo de cambio.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO con los datos del tipo de cambio creado.</returns>
    public async Task<TipoCambioDto> GuardarAsync(
        decimal valor,
        DateOnly fecha,
        CancellationToken cancellationToken = default)
    {
        var tipoCambio = TipoCambio.Crear(valor, fecha);
        await _repository.ReemplazarActivoAsync(tipoCambio, cancellationToken);
        return Mapear(tipoCambio);
    }

    /// <summary>
    /// Obtiene el tipo de cambio actualmente activo.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO del tipo de cambio activo o null si no existe.</returns>
    public async Task<TipoCambioDto?> ObtenerActivoAsync(
        CancellationToken cancellationToken = default)
    {
        var tipoCambio = await _repository.ObtenerActivoAsync(cancellationToken);
        return tipoCambio is null ? null : Mapear(tipoCambio);
    }

    /// <summary>
    /// Obtiene un tipo de cambio por su identificador.
    /// </summary>
    /// <param name="id">Identificador del tipo de cambio.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO del tipo de cambio o null si no existe.</returns>
    public async Task<TipoCambioDto?> ObtenerPorIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var tipoCambio = await _repository.ObtenerPorIdAsync(id, cancellationToken);
        return tipoCambio is null ? null : Mapear(tipoCambio);
    }

    /// <summary>
    /// Actualiza un tipo de cambio existente.
    /// </summary>
    /// <param name="id">Identificador del tipo de cambio.</param>
    /// <param name="valor">Nuevo valor del tipo de cambio.</param>
    /// <param name="fecha">Nueva fecha del tipo de cambio.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO del tipo de cambio actualizado o null si no existe.</returns>
    public async Task<TipoCambioDto?> ActualizarAsync(
        int id,
        decimal valor,
        DateOnly fecha,
        CancellationToken cancellationToken = default)
    {
        var tipoCambio = await _repository.ObtenerPorIdAsync(id, cancellationToken);
        if (tipoCambio is null)
        {
            return null;
        }

        if (valor <= 0)
        {
            throw new DomainException("El valor del tipo de cambio debe ser mayor que cero.");
        }

        tipoCambio.Actualizar(valor, fecha);

        await _repository.GuardarCambiosAsync(cancellationToken);
        return Mapear(tipoCambio);
    }

    /// <summary>
    /// Desactiva un tipo de cambio existente.
    /// </summary>
    /// <param name="id">Identificador del tipo de cambio a desactivar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>True si se desactivó, false si no se encontró.</returns>
    public async Task<bool> EliminarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var tipoCambio = await _repository.ObtenerPorIdAsync(id, cancellationToken);
        if (tipoCambio is null)
        {
            return false;
        }

        tipoCambio.Desactivar();
        await _repository.GuardarCambiosAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Activa un tipo de cambio, desactivando el anterior si existe.
    /// </summary>
    /// <param name="id">Identificador del tipo de cambio a activar.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO del tipo de cambio activado o null si no existe.</returns>
    public async Task<TipoCambioDto?> ActivarAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var tipoCambio = await _repository.ObtenerPorIdAsync(id, cancellationToken);
        if (tipoCambio is null)
        {
            return null;
        }

        var activo = await _repository.ObtenerActivoAsync(cancellationToken);
        if (activo is not null && activo.Id != id)
        {
            activo.Desactivar();
        }

        tipoCambio.Activar();
        await _repository.GuardarCambiosAsync(cancellationToken);
        return Mapear(tipoCambio);
    }

    /// <summary>
    /// Lista todos los tipos de cambio con ordenamiento y paginación.
    /// </summary>
    /// <param name="ordenarPor">Campo de ordenamiento (fecha o valor).</param>
    /// <param name="descendente">Indica si el orden es descendente.</param>
    /// <param name="pagina">Número de página.</param>
    /// <param name="tamanoPagina">Tamaño de la página.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Página de resultados con los tipos de cambio encontrados.</returns>
    public async Task<PaginaResultado<TipoCambioDto>> ListarAsync(
        string ordenarPor = "fecha",
        bool descendente = false,
        int pagina = 1,
        int tamanoPagina = 20,
        CancellationToken cancellationToken = default)
    {
        ValidarConsulta(ordenarPor, pagina, tamanoPagina);

        var tipos = await _repository.ListarTodosAsync(cancellationToken);

        IEnumerable<TipoCambio> ordenados = (ordenarPor.ToLowerInvariant(), descendente) switch
        {
            ("valor", false) => tipos.OrderBy(tipo => tipo.Valor),
            ("valor", true) => tipos.OrderByDescending(tipo => tipo.Valor),
            ("fecha", true) => tipos.OrderByDescending(tipo => tipo.Fecha),
            _ => tipos.OrderBy(tipo => tipo.Fecha)
        };

        var todos = ordenados.ToArray();
        var items = todos
            .Skip((pagina - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .Select(Mapear)
            .ToArray();

        return new PaginaResultado<TipoCambioDto>(items, todos.Length, pagina, tamanoPagina);
    }

    private static void ValidarConsulta(string ordenarPor, int pagina, int tamanoPagina)
    {
        if (pagina <= 0 || tamanoPagina is <= 0 or > 100)
        {
            throw new DomainException("La paginación solicitada no es válida.");
        }

        if (ordenarPor.ToLowerInvariant() is not ("fecha" or "valor"))
        {
            throw new DomainException("El campo de ordenamiento no es válido.");
        }
    }

    private static TipoCambioDto Mapear(TipoCambio tipoCambio) => new(
        tipoCambio.Id,
        tipoCambio.MonedaOrigen,
        tipoCambio.MonedaDestino,
        tipoCambio.Valor,
        tipoCambio.Fecha,
        tipoCambio.Activo);
}
