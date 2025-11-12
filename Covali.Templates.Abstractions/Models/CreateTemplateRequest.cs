namespace Covali.Templates.Abstractions.Models;

/// <summary>
/// Request model for creating a new template.
/// </summary>
public sealed class CreateTemplateRequest
{
    public required Guid OwnerId { get; init; }
    public required OwnerType OwnerType { get; init; }
    public required string TemplateType { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required object Body { get; init; }
}