using System.Text.Json;
using Azure.Messaging.ServiceBus;
using GDPR.Application.Actions;
using GDPR.Application.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace GDPR.Triggers;

public sealed class GdprAcceptanceServiceBusFunction
{
    private readonly ILogger<GdprAcceptanceServiceBusFunction> _logger;
    private readonly ProcessGdprAcceptanceAction _processGdprAcceptanceAction;

    public GdprAcceptanceServiceBusFunction(
        ILogger<GdprAcceptanceServiceBusFunction> logger,
        ProcessGdprAcceptanceAction processGdprAcceptanceAction)
    {
        _logger = logger;
        _processGdprAcceptanceAction = processGdprAcceptanceAction;
    }

    [Function("ProcessGdprAcceptance")]
    public async Task Run(
        [ServiceBusTrigger("gdpr-acceptance", Connection = "ServiceBusConnection")] ServiceBusReceivedMessage message,
        CancellationToken cancellationToken)
    {
        var payload = message.Body.ToObjectFromJson<GdprAcceptanceMessage>(new JsonSerializerOptions(JsonSerializerDefaults.Web))
                      ?? throw new InvalidOperationException("Message body is required.");

        payload.MessageId = string.IsNullOrWhiteSpace(payload.MessageId) ? message.MessageId : payload.MessageId;

        var result = await _processGdprAcceptanceAction.ExecuteAsync(payload, cancellationToken);

        if (result.Processed)
        {
            _logger.LogInformation("Processed GDPR acceptance message {MessageId}.", result.MessageId);
            return;
        }

        _logger.LogInformation("Skipped GDPR acceptance message {MessageId}: {Detail}", result.MessageId, result.Detail);
    }
}
