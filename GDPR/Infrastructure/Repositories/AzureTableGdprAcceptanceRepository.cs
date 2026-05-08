using Azure;
using Azure.Data.Tables;
using GDPR.Domain.Entities;
using GDPR.Domain.Repositories;
using GDPR.Infrastructure.Configuration;
using GDPR.Infrastructure.Persistence;
using Microsoft.Extensions.Options;

namespace GDPR.Infrastructure.Repositories;

public sealed class AzureTableGdprAcceptanceRepository : IGdprAcceptanceRepository
{
    private readonly TableClient _tableClient;

    public AzureTableGdprAcceptanceRepository(TableServiceClient serviceClient, IOptions<TableStorageOptions> options)
    {
        _tableClient = serviceClient.GetTableClient(options.Value.GdprAcceptancesTableName);
        _tableClient.CreateIfNotExists();
    }

    public async Task AddAsync(GdprAcceptance acceptance, CancellationToken cancellationToken)
    {
        try
        {
            await _tableClient.AddEntityAsync(ToEntity(acceptance), cancellationToken);
        }
        catch (RequestFailedException exception) when (exception.Status == 409)
        {
            throw new InvalidOperationException($"Message '{acceptance.MessageId}' has already been processed.", exception);
        }
    }

    public async Task<bool> HasProcessedMessageAsync(string messageId, CancellationToken cancellationToken)
    {
        try
        {
            await _tableClient.GetEntityAsync<GdprAcceptanceTableEntity>(
                GdprAcceptanceTableEntity.PartitionKeyValue,
                messageId,
                cancellationToken: cancellationToken);

            return true;
        }
        catch (RequestFailedException exception) when (exception.Status == 404)
        {
            return false;
        }
    }

    private static GdprAcceptanceTableEntity ToEntity(GdprAcceptance acceptance)
    {
        return new GdprAcceptanceTableEntity
        {
            RowKey = acceptance.MessageId,
            UserId = acceptance.UserId,
            GdprDocumentId = acceptance.GdprDocumentId,
            AcceptedAt = acceptance.AcceptedAt
        };
    }
}
