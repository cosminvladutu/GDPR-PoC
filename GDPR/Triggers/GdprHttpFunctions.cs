using System.Net;
using GDPR.Application.Actions;
using GDPR.Application.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace GDPR.Triggers;

public sealed class GdprHttpFunctions
{
    private readonly CreateGdprDocumentAction _createGdprDocumentAction;
    private readonly GetGdprDocumentAction _getGdprDocumentAction;
    private readonly ListGdprDocumentsAction _listGdprDocumentsAction;
    private readonly UpdateGdprDocumentAction _updateGdprDocumentAction;

    public GdprHttpFunctions(
        CreateGdprDocumentAction createGdprDocumentAction,
        GetGdprDocumentAction getGdprDocumentAction,
        ListGdprDocumentsAction listGdprDocumentsAction,
        UpdateGdprDocumentAction updateGdprDocumentAction)
    {
        _createGdprDocumentAction = createGdprDocumentAction;
        _getGdprDocumentAction = getGdprDocumentAction;
        _listGdprDocumentsAction = listGdprDocumentsAction;
        _updateGdprDocumentAction = updateGdprDocumentAction;
    }

    [OpenApiOperation(operationId: "CreateGdprDocument", tags: new[] { "GdprDocuments" }, Summary = "Create a GDPR document", Description = "Creates a new GDPR document in Azure Table Storage.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiRequestBody("application/json", typeof(CreateGdprDocumentRequest), Required = true, Description = "The GDPR document to create.")]
    [OpenApiResponseWithBody(HttpStatusCode.Created, "application/json", typeof(GdprDocumentResponse), Summary = "Created", Description = "The created GDPR document.")]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(ApiErrorResponse), Summary = "Bad request", Description = "The request payload was invalid.")]
    [OpenApiResponseWithBody(HttpStatusCode.Conflict, "application/json", typeof(ApiErrorResponse), Summary = "Conflict", Description = "The document id already exists.")]
    [Function("CreateGdprDocument")]
    public async Task<HttpResponseData> Create(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "gdpr-documents")] HttpRequestData request,
        CancellationToken cancellationToken)
    {
        var payload = await request.ReadFromJsonAsync<CreateGdprDocumentRequest>(cancellationToken);

        if (payload is null)
        {
            return await HttpResponseFactory.CreateErrorAsync(request, HttpStatusCode.BadRequest, "Request body is required.", cancellationToken);
        }

        try
        {
            var document = await _createGdprDocumentAction.ExecuteAsync(payload, cancellationToken);
            return await HttpResponseFactory.CreateJsonAsync(request, HttpStatusCode.Created, document, cancellationToken);
        }
        catch (ArgumentException exception)
        {
            return await HttpResponseFactory.CreateErrorAsync(request, HttpStatusCode.BadRequest, exception.Message, cancellationToken);
        }
        catch (InvalidOperationException exception)
        {
            return await HttpResponseFactory.CreateErrorAsync(request, HttpStatusCode.Conflict, exception.Message, cancellationToken);
        }
    }

    [OpenApiOperation(operationId: "GetGdprDocument", tags: new[] { "GdprDocuments" }, Summary = "Get a GDPR document", Description = "Gets a GDPR document by id.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter("id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Document id", Description = "The GDPR document id.")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(GdprDocumentResponse), Summary = "OK", Description = "The requested GDPR document.")]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, "application/json", typeof(ApiErrorResponse), Summary = "Not found", Description = "The document was not found.")]
    [Function("GetGdprDocument")]
    public async Task<HttpResponseData> Get(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "gdpr-documents/{id}")] HttpRequestData request,
        string id,
        CancellationToken cancellationToken)
    {
        try
        {
            var document = await _getGdprDocumentAction.ExecuteAsync(id, cancellationToken);
            return await HttpResponseFactory.CreateJsonAsync(request, HttpStatusCode.OK, document, cancellationToken);
        }
        catch (KeyNotFoundException exception)
        {
            return await HttpResponseFactory.CreateErrorAsync(request, HttpStatusCode.NotFound, exception.Message, cancellationToken);
        }
    }

    [OpenApiOperation(operationId: "ListGdprDocuments", tags: new[] { "GdprDocuments" }, Summary = "List GDPR documents", Description = "Lists all GDPR documents.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(GdprDocumentResponse[]), Summary = "OK", Description = "The GDPR documents.")]
    [Function("ListGdprDocuments")]
    public async Task<HttpResponseData> List(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "gdpr-documents")] HttpRequestData request,
        CancellationToken cancellationToken)
    {
        var documents = await _listGdprDocumentsAction.ExecuteAsync(cancellationToken);
        return await HttpResponseFactory.CreateJsonAsync(request, HttpStatusCode.OK, documents, cancellationToken);
    }

    [OpenApiOperation(operationId: "UpdateGdprDocument", tags: new[] { "GdprDocuments" }, Summary = "Update a GDPR document", Description = "Updates the content of an existing GDPR document and increments its version.", Visibility = OpenApiVisibilityType.Important)]
    [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
    [OpenApiParameter("id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Summary = "Document id", Description = "The GDPR document id.")]
    [OpenApiRequestBody("application/json", typeof(UpdateGdprDocumentRequest), Required = true, Description = "The GDPR document update request.")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(GdprDocumentResponse), Summary = "OK", Description = "The updated GDPR document.")]
    [OpenApiResponseWithBody(HttpStatusCode.BadRequest, "application/json", typeof(ApiErrorResponse), Summary = "Bad request", Description = "The request payload was invalid.")]
    [OpenApiResponseWithBody(HttpStatusCode.NotFound, "application/json", typeof(ApiErrorResponse), Summary = "Not found", Description = "The document was not found.")]
    [Function("UpdateGdprDocument")]
    public async Task<HttpResponseData> Update(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "gdpr-documents/{id}")] HttpRequestData request,
        string id,
        CancellationToken cancellationToken)
    {
        var payload = await request.ReadFromJsonAsync<UpdateGdprDocumentRequest>(cancellationToken);

        if (payload is null)
        {
            return await HttpResponseFactory.CreateErrorAsync(request, HttpStatusCode.BadRequest, "Request body is required.", cancellationToken);
        }

        try
        {
            var document = await _updateGdprDocumentAction.ExecuteAsync(id, payload, cancellationToken);
            return await HttpResponseFactory.CreateJsonAsync(request, HttpStatusCode.OK, document, cancellationToken);
        }
        catch (ArgumentException exception)
        {
            return await HttpResponseFactory.CreateErrorAsync(request, HttpStatusCode.BadRequest, exception.Message, cancellationToken);
        }
        catch (KeyNotFoundException exception)
        {
            return await HttpResponseFactory.CreateErrorAsync(request, HttpStatusCode.NotFound, exception.Message, cancellationToken);
        }
    }
}
