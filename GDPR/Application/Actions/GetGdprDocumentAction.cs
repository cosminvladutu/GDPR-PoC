using GDPR.Application.Contracts;
using GDPR.Domain.Repositories;

namespace GDPR.Application.Actions;

public sealed class GetGdprDocumentAction
{
    private readonly IGdprDocumentRepository _repository;

    public GetGdprDocumentAction(IGdprDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<GdprDocumentResponse> ExecuteAsync(string id, CancellationToken cancellationToken)
    {
        var document = await _repository.GetByIdAsync(id, cancellationToken);

        if (document is null)
        {
            throw new KeyNotFoundException($"GDPR document '{id}' was not found.");
        }

        return GdprDocumentResponse.FromDomain(document);
    }
}
