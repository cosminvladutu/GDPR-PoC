namespace GDPR.Application.Contracts;

public sealed class GdprAcceptanceMessage
{
    public string? MessageId { get; set; }

    public string? UserId { get; set; }

    public string? GdprDocumentId { get; set; }
}
