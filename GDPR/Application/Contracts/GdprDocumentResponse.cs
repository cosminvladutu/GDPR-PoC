using GDPR.Domain.Entities;

namespace GDPR.Application.Contracts;

public sealed record GdprDocumentResponse(string Id, int Version, string Content, DateTimeOffset CreatedAt)
{
    public static GdprDocumentResponse FromDomain(GdprDocument document)
    {
        return new GdprDocumentResponse(document.Id, document.Version, document.Content, document.CreatedAt);
    }
}
