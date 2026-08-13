namespace Licitaciones.Domain.Common;

public interface IClock
{
    DateTimeOffset UtcNow();
}
