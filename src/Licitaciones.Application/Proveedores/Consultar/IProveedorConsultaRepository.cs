using Licitaciones.Domain.Proveedores;

namespace Licitaciones.Application.Proveedores.Consultar;

public interface IProveedorConsultaRepository
{
    Task<Proveedor?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<Proveedor?> ObtenerHistoricoPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PaginaProveedores> ListarAsync(
        ConsultarProveedoresRequest consulta,
        CancellationToken cancellationToken = default);

    Task<PaginaProveedores> ListarHistoricoAsync(
        ConsultarProveedoresRequest consulta,
        CancellationToken cancellationToken = default);
}
