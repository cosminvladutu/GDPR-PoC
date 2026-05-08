using GDPR.Domain.Entities;

namespace GDPR.Domain.Repositories;

public interface IGdprDocumentRepository
{
    Task<GdprDocument?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<GdprDocument>> ListAsync(CancellationToken cancellationToken);

    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken);

    Task AddAsync(GdprDocument document, CancellationToken cancellationToken);

    Task UpdateAsync(GdprDocument document, CancellationToken cancellationToken);
}
