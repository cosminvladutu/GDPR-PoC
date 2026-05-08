using GDPR.Application.Contracts;
using GDPR.Domain.Repositories;

namespace GDPR.Application.Actions;

public sealed class UpdateGdprDocumentAction
{
    private readonly IGdprDocumentRepository _repository;

    public UpdateGdprDocumentAction(IGdprDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<GdprDocumentResponse> ExecuteAsync(string id, UpdateGdprDocumentRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var document = await _repository.GetByIdAsync(id, cancellationToken);

        if (document is null)
        {
            throw new KeyNotFoundException($"GDPR document '{id}' was not found.");
        }

        document.UpdateContent(request.Content ?? string.Empty);

        await _repository.UpdateAsync(document, cancellationToken);

        return GdprDocumentResponse.FromDomain(document);
    }
}
