using GDPR.Application.Abstractions;
using GDPR.Application.Contracts;
using GDPR.Domain.Entities;
using GDPR.Domain.Repositories;

namespace GDPR.Application.Actions;

public sealed class CreateGdprDocumentAction
{
    private readonly ISystemClock _clock;
    private readonly IGdprDocumentRepository _repository;

    public CreateGdprDocumentAction(ISystemClock clock, IGdprDocumentRepository repository)
    {
        _clock = clock;
        _repository = repository;
    }

    public async Task<GdprDocumentResponse> ExecuteAsync(CreateGdprDocumentRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var id = string.IsNullOrWhiteSpace(request.Id) ? Guid.NewGuid().ToString("N") : request.Id.Trim();
        var content = request.Content ?? string.Empty;

        if (await _repository.ExistsAsync(id, cancellationToken))
        {
            throw new InvalidOperationException($"A GDPR document with id '{id}' already exists.");
        }

        var document = GdprDocument.Create(id, content, _clock.UtcNow);

        await _repository.AddAsync(document, cancellationToken);

        return GdprDocumentResponse.FromDomain(document);
    }
}
