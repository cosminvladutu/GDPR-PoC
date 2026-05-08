using GDPR.Application.Abstractions;
using GDPR.Application.Contracts;
using GDPR.Domain.Entities;
using GDPR.Domain.Repositories;

namespace GDPR.Application.Actions;

public sealed class ProcessGdprAcceptanceAction
{
    private readonly IGdprAcceptanceRepository _acceptanceRepository;
    private readonly ISystemClock _clock;
    private readonly IGdprDocumentRepository _documentRepository;

    public ProcessGdprAcceptanceAction(
        IGdprAcceptanceRepository acceptanceRepository,
        ISystemClock clock,
        IGdprDocumentRepository documentRepository)
    {
        _acceptanceRepository = acceptanceRepository;
        _clock = clock;
        _documentRepository = documentRepository;
    }

    public async Task<ProcessGdprAcceptanceResult> ExecuteAsync(GdprAcceptanceMessage message, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);

        var messageId = EnsureValue(message.MessageId, nameof(message.MessageId), "MessageId is required.");
        var userId = EnsureValue(message.UserId, nameof(message.UserId), "UserId is required.");
        var gdprDocumentId = EnsureValue(message.GdprDocumentId, nameof(message.GdprDocumentId), "GdprDocumentId is required.");

        if (await _acceptanceRepository.HasProcessedMessageAsync(messageId, cancellationToken))
        {
            return new ProcessGdprAcceptanceResult(messageId, false, "Duplicate message detected. Skipping.");
        }

        var document = await _documentRepository.GetByIdAsync(gdprDocumentId, cancellationToken);

        if (document is null)
        {
            throw new KeyNotFoundException($"GDPR document '{gdprDocumentId}' was not found.");
        }

        var acceptance = GdprAcceptance.Create(messageId, userId, gdprDocumentId, _clock.UtcNow);

        await _acceptanceRepository.AddAsync(acceptance, cancellationToken);

        return new ProcessGdprAcceptanceResult(messageId, true, "Acceptance recorded.");
    }

    private static string EnsureValue(string? value, string paramName, string message)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(message, paramName);
        }

        return value.Trim();
    }
}
