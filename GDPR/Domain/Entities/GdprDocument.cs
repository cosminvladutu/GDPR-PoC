namespace GDPR.Domain.Entities;

public sealed class GdprDocument
{
    private GdprDocument(string id, int version, string content, DateTimeOffset createdAt)
    {
        Id = EnsureId(id);
        Version = EnsureVersion(version);
        Content = EnsureContent(content);
        CreatedAt = createdAt;
    }

    public string Id { get; }

    public int Version { get; private set; }

    public string Content { get; private set; }

    public DateTimeOffset CreatedAt { get; }

    public static GdprDocument Create(string id, string content, DateTimeOffset createdAt)
    {
        return new GdprDocument(id, 1, content, createdAt);
    }

    public static GdprDocument Rehydrate(string id, int version, string content, DateTimeOffset createdAt)
    {
        return new GdprDocument(id, version, content, createdAt);
    }

    public void UpdateContent(string content)
    {
        Content = EnsureContent(content);
        Version += 1;
    }

    private static string EnsureId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Document id is required.", nameof(id));
        }

        return id.Trim();
    }

    private static int EnsureVersion(int version)
    {
        if (version <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(version), "Version must be greater than zero.");
        }

        return version;
    }

    private static string EnsureContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Document content is required.", nameof(content));
        }

        return content.Trim();
    }
}
