using Microsoft.AspNetCore.Mvc;

namespace Licitaciones.Api.Infraestructura;

public static class RespuestaProblema
{
    public const string TipoContenido = "application/problem+json";

    public static ObjectResult Crear(
        HttpContext contexto,
        int estado,
        string titulo,
        string detalle,
        string codigoError)
    {
        ArgumentNullException.ThrowIfNull(contexto);
        ArgumentException.ThrowIfNullOrWhiteSpace(titulo);
        ArgumentException.ThrowIfNullOrWhiteSpace(detalle);
        ArgumentException.ThrowIfNullOrWhiteSpace(codigoError);

        var problema = new ProblemDetails
        {
            Status = estado,
            Title = titulo,
            Detail = detalle,
            Type = $"https://httpstatuses.com/{estado}",
            Instance = contexto.Request.Path
        };
        ContratoProblemasApi.AplicarExtensiones(contexto, problema, codigoError);

        var resultado = estado switch
        {
            StatusCodes.Status400BadRequest => new BadRequestObjectResult(problema),
            StatusCodes.Status404NotFound => new NotFoundObjectResult(problema),
            StatusCodes.Status409Conflict => new ConflictObjectResult(problema),
            StatusCodes.Status422UnprocessableEntity =>
                new UnprocessableEntityObjectResult(problema),
            _ => new ObjectResult(problema)
        };
        resultado.DeclaredType = typeof(ProblemDetails);
        resultado.ContentTypes.Add(TipoContenido);

        return resultado;
    }
}
