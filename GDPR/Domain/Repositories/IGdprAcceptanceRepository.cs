using GDPR.Domain.Entities;

namespace GDPR.Domain.Repositories;

public interface IGdprAcceptanceRepository
{
    Task<bool> HasProcessedMessageAsync(string messageId, CancellationToken cancellationToken);

    Task AddAsync(GdprAcceptance acceptance, CancellationToken cancellationToken);
}
