using Licitaciones.Application.Proveedores.Consultar;
using Licitaciones.Domain.Common;
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
