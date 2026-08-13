using Licitaciones.Domain.Common;

namespace Licitaciones.UnitTests.Common;

public sealed class FixedClock : IClock
{
    public FixedClock(DateTimeOffset utcNow)
    {
        UtcNowValue = utcNow.ToUniversalTime();
    }

    public DateTimeOffset UtcNowValue { get; set; }

    public DateTimeOffset UtcNow() => UtcNowValue;
}
