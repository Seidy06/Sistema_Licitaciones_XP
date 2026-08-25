namespace Licitaciones.Domain.Common;

/// <summary>
/// Excepción que representa una violación de una regla de negocio del dominio.
/// </summary>
public class DomainException : Exception
{
    /// <param name="message">Mensaje descriptivo de la violación de la regla.</param>
    public DomainException(string message)
        : base(message)
    {
    }

    /// <param name="message">Mensaje descriptivo de la violación de la regla.</param>
    /// <param name="code">Código identificador de la regla violada.</param>
    public DomainException(string message, string code)
        : base(message)
    {
        Code = code;
    }

    /// <summary>
    /// Código opcional que identifica la regla de negocio violada.
    /// </summary>
    public string? Code { get; }
}
