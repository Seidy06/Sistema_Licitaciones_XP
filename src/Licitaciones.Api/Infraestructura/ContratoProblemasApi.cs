using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Api.Infraestructura;

public static class ContratoProblemasApi
{
    public const string ClaveCodigoError = "codigoError";
    public const string ClaveCorrelacionId = "correlacionId";

    public static void AplicarExtensiones(
        HttpContext contexto,
        ProblemDetails problema,
        string? codigoError = null)
    {
        problema.Extensions[ClaveCodigoError] =
            string.IsNullOrWhiteSpace(codigoError)
                ? $"error_http_{problema.Status ?? StatusCodes.Status500InternalServerError}"
                : codigoError;
        problema.Extensions[ClaveCorrelacionId] = contexto.TraceIdentifier;
    }
}
