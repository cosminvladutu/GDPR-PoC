namespace GDPR.Application.Contracts;

public sealed class CreateGdprDocumentRequest
{
    public string? Id { get; init; }

    public string? Content { get; init; }
}
