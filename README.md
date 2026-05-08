# Squad Azure Functions PoC

This repository contains a small Proof of Concept created while testing Squad, the multi-agent orchestration tool built around GitHub Copilot.

The goal was not to build a production-ready application, but to explore how AI agents collaborate on a cloud-oriented .NET solution that goes slightly beyond a simple CRUD application.

This PoC was created as part of the article:

> **Testing Squad with Azure Functions and Azure Tables**
>
> You can find the full article on my blog:
> [Cosmin Vladutu](https://cosmin-vladutu.medium.com/)

---

## What This Solution Does

The application simulates a GDPR acceptance workflow using Azure Functions.

It contains:
- HTTP-triggered endpoints for managing GDPR documents
- A Service Bus-triggered function for processing GDPR acceptances
- Azure Table Storage persistence
- OpenAPI / Swagger support
- A DDD + Clean Architecture-inspired structure

The GDPR entity contains:
- `Id`
- `Version`
- `Content`
- `CreatedAt`

The acceptance flow stores:
- `UserId`
- `GdprId`
- `MessageId` (for duplicate detection scenarios)

---

## Why This PoC Exists

Most AI demos focus on:
- simple web applications
- isolated APIs
- or ideal greenfield scenarios

I wanted to test something slightly different:
- multiple trigger types
- asynchronous workflows
- Azure-specific SDKs
- cloud persistence
- and lightweight domain modeling

The goal was to see how well AI agents handle:
- orchestration
- separation of concerns
- Azure Table Storage
- and a more realistic cloud-native workflow

---

## Tech Stack

- .NET 10
- Azure Functions (Isolated Worker)
- Azure Service Bus
- Azure Table Storage
- OpenAPI / Swagger
- DDD-inspired structure
- Clean Architecture-inspired layering

---

## Important Notes

This solution is intentionally a PoC.

It was created to evaluate:
- AI-assisted development workflows
- agent collaboration
- architectural decisions
- and generated code quality

It should **not** be considered production-ready.

Some areas intentionally left simple or identified during the experiment:
- concurrency handling
- retry strategies
- validation improvements
- distributed system edge cases

---

## Interesting Observations

Some things that stood out during the experiment:
- agents worked in parallel on different features
- the solution evolved toward a more DDD-oriented structure over time
- Azure SDK usage was reasonable
- async usage was decent
- the generated code avoided excessive abstractions

One interesting limitation:
- the agents rarely asked clarification questions and mostly focused on execution.

---

## Overview

This repository contains a .NET 10 Azure Functions project that:

- exposes HTTP endpoints for managing GDPR documents
- stores documents in Azure Table Storage
- processes GDPR acceptance messages from an Azure Service Bus queue
- uses dependency injection, application insights telemetry, and JSON serialization configuration

## Project structure

- `GDPR/` - Azure Functions app source
- `GDPR/Triggers/` - function trigger classes for HTTP and Service Bus
- `GDPR/Application/` - application actions, contracts, and abstractions
- `GDPR/Domain/` - domain entities and repository interfaces
- `GDPR/Infrastructure/` - Azure Table Storage repository implementations and configuration

## Requirements

- .NET 10 SDK
- Azure Functions Core Tools
- Azure Storage account or Storage Emulator
- Azure Service Bus namespace for the `gdpr-acceptance` queue

## Configuration

The function app reads configuration from environment variables or `local.settings.json`.

Required settings:

- `AzureWebJobsStorage` - Azure Table Storage connection string
- `FUNCTIONS_WORKER_RUNTIME` - should be `dotnet-isolated`
- `ServiceBusConnection` - Service Bus connection string
- `GdprDocumentsTableName` (optional) - table name for GDPR documents
- `GdprAcceptancesTableName` (optional) - table name for GDPR acceptance records

Default table names are configured in `TableStorageOptions`.

### Local development

The included `local.settings.json` uses the storage emulator:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  }
}
```

For local testing, configure `ServiceBusConnection` in `local.settings.json` or environment variables and ensure the `gdpr-acceptance` queue exists.

## HTTP API

The solution exposes the following HTTP functions:

- `POST /api/gdpr-documents`
  - Creates a GDPR document
  - Request body: `{ "id": "string", "content": "string" }`

- `GET /api/gdpr-documents/{id}`
  - Retrieves a GDPR document by id

- `GET /api/gdpr-documents`
  - Lists all GDPR documents

- `PUT /api/gdpr-documents/{id}`
  - Updates an existing GDPR document
  - Request body: `{ "content": "string" }`

### Example create request

```http
POST /api/gdpr-documents?code=<function_key>
Content-Type: application/json

{
  "id": "document-123",
  "content": "GDPR policy version 1"
}
```

## Service Bus integration

A Service Bus triggered function listens on the `gdpr-acceptance` queue and processes messages of type `GdprAcceptanceMessage`.

Message schema:

```json
{
  "messageId": "string",
  "userId": "string",
  "gdprDocumentId": "string"
}
```

The function logs whether the acceptance message was processed or skipped.

## Running locally

From the repository root:

```powershell
cd GDPR
dotnet build
func host start
```

When running locally, use the function key and `AzureWebJobsStorage`/`ServiceBusConnection` settings to call the HTTP API and push messages into Service Bus.

## Observability

- Application Insights telemetry is configured for the worker service.
- HTTP endpoints use structured JSON serialization with camelCase naming and null-value omission.

## Notes

- The application uses `Azure.Data.Tables` for storage operations and `Azure.Messaging.ServiceBus` for message processing.
- If `AzureWebJobsStorage` is missing, the application throws a startup error.
- The project targets `net10.0` and the Azure Functions isolated worker model.
