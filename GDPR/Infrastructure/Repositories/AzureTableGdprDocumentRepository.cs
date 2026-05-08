using Azure;
using Azure.Data.Tables;
using GDPR.Domain.Entities;
using GDPR.Domain.Repositories;
using GDPR.Infrastructure.Configuration;
using GDPR.Infrastructure.Persistence;
using Microsoft.Extensions.Options;

namespace GDPR.Infrastructure.Repositories;

public sealed class AzureTableGdprDocumentRepository : IGdprDocumentRepository
{
    private readonly TableClient _tableClient;

    public AzureTableGdprDocumentRepository(TableServiceClient serviceClient, IOptions<TableStorageOptions> options)
    {
        _tableClient = serviceClient.GetTableClient(options.Value.GdprDocumentsTableName);
        _tableClient.CreateIfNotExists();
    }

    public async Task AddAsync(GdprDocument document, CancellationToken cancellationToken)
    {
        try
        {
            await _tableClient.AddEntityAsync(ToEntity(document), cancellationToken);
        }
        catch (RequestFailedException exception) when (exception.Status == 409)
        {
            throw new InvalidOperationException($"A GDPR document with id '{document.Id}' already exists.", exception);
        }
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken)
    {
        return await GetEntityOrDefaultAsync(id, cancellationToken) is not null;
    }

    public async Task<GdprDocument?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await GetEntityOrDefaultAsync(id, cancellationToken);
        return entity is null ? null : ToDomain(entity);
    }

    public async Task<IReadOnlyCollection<GdprDocument>> ListAsync(CancellationToken cancellationToken)
    {
        var results = new List<GdprDocument>();

        await foreach (var entity in _tableClient.QueryAsync<GdprDocumentTableEntity>(
                           entity => entity.PartitionKey == GdprDocumentTableEntity.PartitionKeyValue,
                           cancellationToken: cancellationToken))
        {
            results.Add(ToDomain(entity));
        }

        return results
            .OrderByDescending(document => document.CreatedAt)
            .ToArray();
    }

    public async Task UpdateAsync(GdprDocument document, CancellationToken cancellationToken)
    {
        await _tableClient.UpsertEntityAsync(ToEntity(document), TableUpdateMode.Replace, cancellationToken);
    }

    private async Task<GdprDocumentTableEntity?> GetEntityOrDefaultAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _tableClient.GetEntityAsync<GdprDocumentTableEntity>(
                GdprDocumentTableEntity.PartitionKeyValue,
                id,
                cancellationToken: cancellationToken);

            return response.Value;
        }
        catch (RequestFailedException exception) when (exception.Status == 404)
        {
            return null;
        }
    }

    private static GdprDocumentTableEntity ToEntity(GdprDocument document)
    {
        return new GdprDocumentTableEntity
        {
            RowKey = document.Id,
            Version = document.Version,
            Content = document.Content,
            CreatedAt = document.CreatedAt
        };
    }

    private static GdprDocument ToDomain(GdprDocumentTableEntity entity)
    {
        return GdprDocument.Rehydrate(entity.RowKey, entity.Version, entity.Content, entity.CreatedAt);
    }
}
