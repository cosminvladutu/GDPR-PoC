using Azure.Core.Serialization;
using Azure.Data.Tables;
using GDPR.Application.Abstractions;
using GDPR.Application.Actions;
using GDPR.Domain.Repositories;
using GDPR.Infrastructure.Configuration;
using GDPR.Infrastructure.Repositories;
using GDPR.Infrastructure.Time;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.Configure<WorkerOptions>(options =>
{
    var settings = NewtonsoftJsonObjectSerializer.CreateJsonSerializerSettings();
    settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
    settings.NullValueHandling = NullValueHandling.Ignore;

    options.Serializer = new NewtonsoftJsonObjectSerializer(settings);
});

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services
    .AddOptions<TableStorageOptions>()
    .Configure(options =>
    {
        options.ConnectionString = builder.Configuration["AzureWebJobsStorage"] ?? string.Empty;
        options.GdprDocumentsTableName = builder.Configuration["GdprDocumentsTableName"] ?? TableStorageOptions.DefaultDocumentsTableName;
        options.GdprAcceptancesTableName = builder.Configuration["GdprAcceptancesTableName"] ?? TableStorageOptions.DefaultAcceptancesTableName;
    });

builder.Services.AddSingleton(provider =>
{
    var options = provider.GetRequiredService<IOptions<TableStorageOptions>>().Value;

    if (string.IsNullOrWhiteSpace(options.ConnectionString))
    {
        throw new InvalidOperationException("AzureWebJobsStorage configuration is required.");
    }

    return new TableServiceClient(options.ConnectionString);
});

builder.Services.AddSingleton<ISystemClock, SystemClock>();
builder.Services.AddSingleton<IGdprDocumentRepository, AzureTableGdprDocumentRepository>();
builder.Services.AddSingleton<IGdprAcceptanceRepository, AzureTableGdprAcceptanceRepository>();

builder.Services.AddTransient<CreateGdprDocumentAction>();
builder.Services.AddTransient<GetGdprDocumentAction>();
builder.Services.AddTransient<ListGdprDocumentsAction>();
builder.Services.AddTransient<UpdateGdprDocumentAction>();
builder.Services.AddTransient<ProcessGdprAcceptanceAction>();

builder.Build().Run();
