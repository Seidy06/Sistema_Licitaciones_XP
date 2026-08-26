using Licitaciones.Application.Common;
using Licitaciones.Domain.Proveedores;

namespace Licitaciones.Application.Proveedores.Consultar;

/// <summary>
/// Servicio para consultar proveedores activos e históricos con paginación.
/// </summary>
public sealed class ConsultarProveedorService
{
    private readonly IProveedorConsultaRepository _repository;

    public ConsultarProveedorService(IProveedorConsultaRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Obtiene un proveedor activo por su identificador.
    /// </summary>
    /// <param name="id">Identificador del proveedor.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO del proveedor o null si no existe.</returns>
    public async Task<ProveedorDto?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var proveedor = await _repository.ObtenerPorIdAsync(id, cancellationToken);
        return proveedor is null ? null : Mapear(proveedor);
    }

    /// <summary>
    /// Obtiene un proveedor histórico (incluido dado de baja) por su identificador.
    /// </summary>
    /// <param name="id">Identificador del proveedor.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>DTO histórico del proveedor o null si no existe.</returns>
    public async Task<ProveedorHistoricoDto?> ObtenerHistoricoPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var proveedor = await _repository.ObtenerHistoricoPorIdAsync(id, cancellationToken);
        return proveedor is null ? null : MapearHistorico(proveedor);
    }

    /// <summary>
    /// Lista proveedores activos paginados según los filtros de búsqueda.
    /// </summary>
    /// <param name="consulta">Parámetros de filtrado y paginación.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Página de resultados con los proveedores encontrados.</returns>
    public async Task<PaginaResultado<ProveedorDto>> ListarAsync(
        ConsultarProveedoresRequest consulta,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _repository.ListarAsync(consulta, cancellationToken);

        return new PaginaResultado<ProveedorDto>(
            resultado.Items.Select(Mapear).ToArray(),
            resultado.Total,
            consulta.Pagina,
            consulta.TamanoPagina);
    }

    /// <summary>
    /// Lista proveedores históricos paginados según los filtros de búsqueda.
    /// </summary>
    /// <param name="consulta">Parámetros de filtrado y paginación.</param>
    /// <param name="cancellationToken">Token de cancelación.</param>
    /// <returns>Página de resultados con los proveedores históricos encontrados.</returns>
    public async Task<PaginaResultado<ProveedorHistoricoDto>> ListarHistoricoAsync(
        ConsultarProveedoresRequest consulta,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _repository.ListarHistoricoAsync(consulta, cancellationToken);

        return new PaginaResultado<ProveedorHistoricoDto>(
            resultado.Items.Select(MapearHistorico).ToArray(),
            resultado.Total,
            consulta.Pagina,
            consulta.TamanoPagina);
    }

    private static ProveedorDto Mapear(Proveedor proveedor) => new(
        proveedor.Id,
        proveedor.Nombre,
        proveedor.NombreNormalizado,
        proveedor.CreatedAt,
        proveedor.UpdatedAt,
        proveedor.Version);

    private static ProveedorHistoricoDto MapearHistorico(Proveedor proveedor) => new(
        proveedor.Id,
        proveedor.Nombre,
        proveedor.NombreNormalizado,
        proveedor.CreatedAt,
        proveedor.UpdatedAt,
        proveedor.DeletedAt!.Value,
        proveedor.Version);
}
