namespace Covali.Templates.Abstractions.Models;

/// <summary>
/// Data transfer object representing a template.
/// </summary>
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