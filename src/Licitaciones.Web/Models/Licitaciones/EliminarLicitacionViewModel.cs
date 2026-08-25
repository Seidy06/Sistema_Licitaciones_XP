namespace Licitaciones.Web.Models.Licitaciones;

public sealed class EliminarLicitacionViewModel
{
    public Guid Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Titulo { get; init; } = string.Empty;
    public string Estado { get; init; } = string.Empty;
}
