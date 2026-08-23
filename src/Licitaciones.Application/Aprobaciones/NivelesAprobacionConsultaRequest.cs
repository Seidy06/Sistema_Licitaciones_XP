namespace Licitaciones.Application.Aprobaciones;

public sealed record NivelesAprobacionConsultaRequest(
    string? Nombre = null,
    string OrdenarPor = "montoMinimo",
    bool Descendente = false,
    int Pagina = 1,
    int TamanoPagina = 20);
