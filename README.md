# Covali.Templates

[![NuGet](https://img.shields.io/nuget/v/Covali.Templates.svg)](https://www.nuget.org/packages/Covali.Templates)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

A flexible, multi-tenant template storage and management system for .NET applications. Store, retrieve, and track reusable data structures (transactions, filters, reports, etc.) with JSON body storage for maximum flexibility.

## Features

- **Multi-tenant Architecture**: Templates can be owned by Users, Groups, or Organizations
- **Flexible Storage**: Store any JSON-serializable object as template body (up to 1MB)
- **Usage Tracking**: Automatic tracking of template usage frequency
- **Rich Querying**: Filter by owner, type, and sort by usage, name, or date
- **Type-safe API**: Strongly-typed DTOs and entities with nullable reference types
- **Entity Framework Integration**: Built-in EF Core support with abstract DbContext
- **Repository Pattern**: Clean separation of concerns with repository abstraction

## Installation

Install the main package via NuGet:

```bash
dotnet add package Covali.Templates
```

### Package Structure

The library is split into three packages:

- **Covali.Templates**: Core service implementation and repository
- **Covali.Templates.Abstractions**: Interfaces, DTOs, and models (minimal dependencies)
- **Covali.Templates.EntityFramework**: EF Core entities, configurations, and DbContext

## Quick Start

### 1. Define Your DbContext

Create a DbContext that inherits from `AbstractTemplateDbContext`:

```csharp
using Covali.Templates.EntityFramework;
using Microsoft.EntityFrameworkCore;

public class MyDbContext : AbstractTemplateDbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }
}
```

### 2. Register Services

Register the DbContext and Covali.Templates services:

```csharp
using Covali.Templates.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register your DbContext
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register Covali.Templates services
builder.Services.AddTemplateDbContext<MyDbContext>();
builder.Services.AddCovaliTemplates();
```

### 3. Run Migrations

Create and apply the migration to set up the database schema:

```bash
dotnet ef migrations add AddTemplates
dotnet ef database update
```

### 4. Use the Template Service

Inject `ITemplateService` and start managing templates:

```csharp
using Covali.Templates.Abstractions;
using Covali.Templates.Abstractions.Models;

public class MyService
{
    private readonly ITemplateService _templateService;

    public MyService(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    public async Task CreateAndUseTemplate(Guid userId)
    {
        // Create a template
        var request = new CreateTemplateRequest
        {
            OwnerId = userId,
            OwnerType = OwnerType.User,
            TemplateType = "transaction",
            Name = "Monthly Rent",
            Description = "Standard rent payment template",
            Body = new
            {
                Category = "Housing",
                Amount = 1200.00,
                Payee = "Property Manager",
                IsRecurring = true
            }
        };

        var template = await _templateService.AddAsync(request);

        // Retrieve template by ID
        var retrieved = await _templateService.GetByIdAsync(template.Id);

        // Get all templates for a user
        var userTemplates = await _templateService.GetByOwnerAsync(
            ownerId: userId,
            ownerType: OwnerType.User,
            templateType: "transaction",
            sortBy: TemplateSortBy.UsageCount,
            page: 1,
            pageSize: 10
        );

        // Track usage
        await _templateService.IncrementUsageAsync(template.Id);

        // Get most used templates
        var mostUsed = await _templateService.GetMostUsedAsync(
            ownerId: userId,
            ownerType: OwnerType.User,
            templateType: null, // All types
            limit: 5
        );
    }
}
```

## API Reference

### ITemplateService

The main service interface for template operations.

#### Methods

##### GetByIdAsync
```csharp
Task<TemplateDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
```
Retrieves a template by its unique identifier.

##### GetByOwnerAsync
```csharp
Task<IReadOnlyCollection<TemplateDto>> GetByOwnerAsync(
    Guid ownerId,
    OwnerType ownerType,
    string? templateType,
    TemplateSortBy sortBy,
    int page,
    int pageSize,
    CancellationToken ct = default
)
```
Retrieves paginated templates for a specific owner with optional filtering and sorting.

##### GetMostUsedAsync
```csharp
Task<IReadOnlyCollection<TemplateDto>> GetMostUsedAsync(
    Guid ownerId,
    OwnerType ownerType,
    string? templateType,
    int limit,
    CancellationToken ct = default
)
```
Retrieves the most frequently used templates for an owner.

##### AddAsync
```csharp
Task<TemplateDto> AddAsync(CreateTemplateRequest request, CancellationToken ct = default)
```
Creates a new template. Throws `InvalidOperationException` if a template with the same name already exists for the owner or if body size exceeds 1MB.

##### UpdateAsync
```csharp
Task<TemplateDto> UpdateAsync(Guid id, UpdateTemplateRequest request, CancellationToken ct = default)
```
Updates an existing template. Throws `InvalidOperationException` if template not found or body size exceeds 1MB.

##### DeleteAsync
```csharp
Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
```
Deletes a template. Returns `true` if deleted, `false` if not found.

##### IncrementUsageAsync
```csharp
Task<int> IncrementUsageAsync(Guid id, CancellationToken ct = default)
```
Increments the usage counter for a template. Returns the new usage count.

##### ExistsAsync
```csharp
Task<bool> ExistsAsync(
    Guid ownerId,
    OwnerType ownerType,
    string templateType,
    string name,
    CancellationToken ct = default
)
```
Checks if a template with the specified name already exists for the owner.

### Data Models

#### TemplateDto

```csharp
public sealed class TemplateDto
{
    public required Guid Id { get; init; }
    public required Guid OwnerId { get; init; }
    public required OwnerType OwnerType { get; init; }
    public required string TemplateType { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required object Body { get; init; }
    public int UsageCount { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
}
```

#### CreateTemplateRequest

```csharp
public sealed class CreateTemplateRequest
{
    public required Guid OwnerId { get; init; }
    public required OwnerType OwnerType { get; init; }
    public required string TemplateType { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required object Body { get; init; }
}
```

#### UpdateTemplateRequest

```csharp
public sealed class UpdateTemplateRequest
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required object Body { get; init; }
}
```

### Enums

#### OwnerType

Defines the type of entity that owns a template:

- `User = 1`: Template owned by an individual user
- `Group = 2`: Template owned by a group (team, family)
- `Organization = 3`: Template owned by an organization

#### TemplateSortBy

Sorting options for template queries:

- `UsageCount = 1`: Sort by usage count (descending)
- `Name = 2`: Sort by template name (ascending)
- `CreatedAt = 3`: Sort by creation date (descending)
- `UpdatedAt = 4`: Sort by last update date (descending)

## Database Schema

The `Template` entity includes:

| Column | Type | Constraints | Description |
|--------|------|-------------|-------------|
| Id | Guid | Primary Key | Unique template identifier |
| OwnerId | Guid | Required, Indexed | Owner entity ID |
| OwnerType | String | Required, Indexed, Max 50 chars | Owner type (stored as string enum) |
| TemplateType | String | Required, Indexed, Max 50 chars | Application-defined template category |
| Name | String | Required, Max 100 chars | Template display name |
| Description | String | Optional, Max 500 chars | Human-readable description |
| Body | String | Required, Max 1MB | JSON-serialized template content |
| UsageCount | Int | Required, Default 0 | Usage frequency counter |
| CreatedAt | DateTimeOffset | Required | Creation timestamp |
| UpdatedAt | DateTimeOffset | Optional | Last modification timestamp |
| DeletedAt | DateTimeOffset | Optional | Soft delete timestamp |

### Indexes

The following indexes are automatically created:

- `(OwnerId, OwnerType, TemplateType)`: Optimizes owner-based queries
- `(OwnerId, OwnerType, TemplateType, Name)`: Ensures uniqueness and optimizes name lookups

## Advanced Usage

### Custom Template Types

Templates are completely flexible - define your own template types based on your application needs:

```csharp
// Transaction template
Body = new
{
    Category = "Food",
    Amount = 50.00,
    Tags = new[] { "groceries", "weekly" }
}

// Filter template
Body = new
{
    DateRange = "last-30-days",
    Categories = new[] { "Food", "Transport" },
    MinAmount = 10.00
}

// Report template
Body = new
{
    ReportType = "monthly-summary",
    Grouping = "category",
    Charts = new[] { "pie", "bar" }
}
```

### Direct Repository Access

For advanced scenarios, you can use `ITemplateRepository` directly:

```csharp
using Covali.Templates.Repositories;

public class AdvancedService
{
    private readonly ITemplateRepository _repository;

    public AdvancedService(ITemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task CustomQuery()
    {
        // Repository works with Template entities, not DTOs
        var templates = await _repository.GetByOwnerAsync(
            ownerId: userId,
            ownerType: OwnerType.User,
            templateType: "custom",
            sortBy: TemplateSortBy.CreatedAt,
            page: 1,
            pageSize: 20
        );
    }
}
```

## Validation and Error Handling

The service performs validation and throws `InvalidOperationException` in these cases:

- Creating a template with a name that already exists for the owner
- Updating/retrieving a template that doesn't exist
- Template body exceeds 1MB when serialized to JSON

Always wrap service calls in try-catch blocks:

```csharp
try
{
    var template = await _templateService.AddAsync(request);
}
catch (InvalidOperationException ex)
{
    // Handle duplicate name or size limit
    logger.LogError(ex, "Failed to create template");
}
```

## Best Practices

1. **Template Types**: Use consistent, kebab-case naming for template types (e.g., `"transaction"`, `"report-filter"`)
2. **Body Size**: Keep template bodies under 100KB for optimal performance (1MB is the hard limit)
3. **Usage Tracking**: Call `IncrementUsageAsync` when a template is actually used, not just retrieved
4. **Pagination**: Use reasonable page sizes (10-50) to avoid performance issues
5. **Type Safety**: Define strongly-typed classes for your template bodies and deserialize accordingly

## Framework Requirements

- .NET 9.0 or later
- Entity Framework Core 9.0 or later

## License

This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/cva-pasha/Covali.Templates).
