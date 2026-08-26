namespace Licitaciones.Web.Models.NivelesAprobacion;

public sealed class DetalleNivelAprobacionViewModel
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public decimal MontoMinimo { get; init; }
    public decimal? MontoMaximo { get; init; }
    public bool Activo { get; init; }
}
