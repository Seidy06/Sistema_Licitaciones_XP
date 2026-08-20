namespace Licitaciones.Domain.Common;

public class DomainException : Exception
{
    public DomainException(string message)
        : base(message)
    {
    }

    public DomainException(string message, string code)
        : base(message)
    {
        Code = code;
    }

    public string? Code { get; }
}
