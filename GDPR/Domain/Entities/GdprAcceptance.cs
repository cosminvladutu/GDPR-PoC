namespace GDPR.Domain.Entities;

public sealed class GdprAcceptance
{
    private GdprAcceptance(string messageId, string userId, string gdprDocumentId, DateTimeOffset acceptedAt)
    {
        MessageId = EnsureValue(messageId, nameof(messageId), "MessageId is required.");
        UserId = EnsureValue(userId, nameof(userId), "UserId is required.");
        GdprDocumentId = EnsureValue(gdprDocumentId, nameof(gdprDocumentId), "GdprDocumentId is required.");
        AcceptedAt = acceptedAt;
    }

    public string MessageId { get; }

    public string UserId { get; }

    public string GdprDocumentId { get; }

    public DateTimeOffset AcceptedAt { get; }

    public static GdprAcceptance Create(string messageId, string userId, string gdprDocumentId, DateTimeOffset acceptedAt)
    {
        return new GdprAcceptance(messageId, userId, gdprDocumentId, acceptedAt);
    }

    private static string EnsureValue(string value, string paramName, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message, paramName);
        }

        return value.Trim();
    }
}
