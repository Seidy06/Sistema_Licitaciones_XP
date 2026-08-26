using Licitaciones.Domain.Proveedores;

namespace Licitaciones.Application.Proveedores.Consultar;

/// <summary>
/// Puerto de salida para la consulta de proveedores en el contexto de solo lectura.
/// </summary>
public interface IProveedorConsultaRepository
{
    /// <summary>
    /// Obtiene un proveedor activo por su identificador.
    /// </summary>
    Task<Proveedor?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtiene un proveedor por su identificador incluyendo datos históricos.
    /// </summary>
    Task<Proveedor?> ObtenerHistoricoPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista proveedores activos según los filtros de búsqueda.
    /// </summary>
    Task<PaginaProveedores> ListarAsync(
        ConsultarProveedoresRequest consulta,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista proveedores históricos (incluidos dados de baja) según los filtros de búsqueda.
    /// </summary>
    Task<PaginaProveedores> ListarHistoricoAsync(
        ConsultarProveedoresRequest consulta,
        CancellationToken cancellationToken = default);
}
