using Azure;
using Azure.Data.Tables;

namespace GDPR.Infrastructure.Persistence;

public sealed class GdprDocumentTableEntity : ITableEntity
{
    public const string PartitionKeyValue = "GDPR_DOCUMENT";

    public string PartitionKey { get; set; } = PartitionKeyValue;

    public string RowKey { get; set; } = string.Empty;

    public int Version { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }
}
