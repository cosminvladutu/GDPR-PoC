namespace GDPR.Infrastructure.Configuration;

public sealed class TableStorageOptions
{
    public const string DefaultDocumentsTableName = "GdprDocuments";
    public const string DefaultAcceptancesTableName = "GdprAcceptances";

    public string ConnectionString { get; set; } = string.Empty;

    public string GdprDocumentsTableName { get; set; } = DefaultDocumentsTableName;

    public string GdprAcceptancesTableName { get; set; } = DefaultAcceptancesTableName;
}
