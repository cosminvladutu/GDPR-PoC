using System.Net;
using GDPR.Application.Contracts;
using Microsoft.Azure.Functions.Worker.Http;

namespace GDPR.Triggers;

internal static class HttpResponseFactory
{
    public static async Task<HttpResponseData> CreateJsonAsync<T>(
        HttpRequestData request,
        HttpStatusCode statusCode,
        T payload,
        CancellationToken cancellationToken)
    {
        var response = request.CreateResponse(statusCode);
        await response.WriteAsJsonAsync(payload, cancellationToken);
        return response;
    }

    public static Task<HttpResponseData> CreateErrorAsync(
        HttpRequestData request,
        HttpStatusCode statusCode,
        string message,
        CancellationToken cancellationToken)
    {
        return CreateJsonAsync(request, statusCode, new ApiErrorResponse(message), cancellationToken);
    }
}
