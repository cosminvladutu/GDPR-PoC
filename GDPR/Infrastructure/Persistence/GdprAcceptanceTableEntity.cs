using Azure;
using Azure.Data.Tables;

namespace GDPR.Infrastructure.Persistence;

public sealed class GdprAcceptanceTableEntity : ITableEntity
{
    public const string PartitionKeyValue = "GDPR_ACCEPTANCE";

    public string PartitionKey { get; set; } = PartitionKeyValue;

    public string RowKey { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string GdprDocumentId { get; set; } = string.Empty;

    public DateTimeOffset AcceptedAt { get; set; }

    public DateTimeOffset? Timestamp { get; set; }

    public ETag ETag { get; set; }
}
