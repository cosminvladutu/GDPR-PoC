using GDPR.Application.Abstractions;

namespace GDPR.Infrastructure.Time;

public sealed class SystemClock : ISystemClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
