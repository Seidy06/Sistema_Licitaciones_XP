namespace Licitaciones.Application.Aprobaciones;

/// <summary>
/// Parámetros de consulta para filtrar y paginar niveles de aprobación.
/// </summary>
public sealed record NivelesAprobacionConsultaRequest(
    string? Nombre = null,
    string OrdenarPor = "montoMinimo",
    bool Descendente = false,
    int Pagina = 1,
    int TamanoPagina = 20);
