namespace GDPR.Application.Abstractions;

public interface ISystemClock
{
    DateTimeOffset UtcNow { get; }
}
