namespace Licitaciones.Web.Models.Licitaciones;

public sealed class DetalleLicitacionViewModel
{
    public Guid Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Titulo { get; init; } = string.Empty;
    public decimal Presupuesto { get; init; }
    public DateTimeOffset FechaCierre { get; init; }
    public LicitacionMejorOfertaViewModel? MejorOferta { get; init; }
    public string? MensajeMejorOferta { get; init; }
    public LicitacionNivelAprobacionViewModel? NivelAprobacion { get; init; }
}

public sealed class LicitacionMejorOfertaViewModel
{
    public Guid Id { get; init; }
    public decimal Monto { get; init; }
    public decimal AhorroPorcentaje { get; init; }
    public string Clasificacion { get; init; } = string.Empty;
}

public sealed class LicitacionNivelAprobacionViewModel
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
}
