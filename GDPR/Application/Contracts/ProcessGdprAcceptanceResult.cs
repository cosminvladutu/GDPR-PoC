namespace GDPR.Application.Contracts;

public sealed record ProcessGdprAcceptanceResult(string MessageId, bool Processed, string Detail);
