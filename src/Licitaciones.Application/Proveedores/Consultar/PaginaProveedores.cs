using Licitaciones.Domain.Proveedores;

namespace Licitaciones.Application.Proveedores.Consultar;

/// <summary>
/// Resultado paginado de la consulta de proveedores.
/// </summary>
public sealed record PaginaProveedores
{
    public PaginaProveedores(IReadOnlyList<Proveedor> items, int total)
    {
        Items = items;
        Total = total;
    }

    public IReadOnlyList<Proveedor> Items { get; }
    public int Total { get; }
}
