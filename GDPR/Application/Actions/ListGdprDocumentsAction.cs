using GDPR.Application.Contracts;
using GDPR.Domain.Repositories;

namespace GDPR.Application.Actions;

public sealed class ListGdprDocumentsAction
{
    private readonly IGdprDocumentRepository _repository;

    public ListGdprDocumentsAction(IGdprDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyCollection<GdprDocumentResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var documents = await _repository.ListAsync(cancellationToken);

        return documents
            .Select(GdprDocumentResponse.FromDomain)
            .ToArray();
    }
}
