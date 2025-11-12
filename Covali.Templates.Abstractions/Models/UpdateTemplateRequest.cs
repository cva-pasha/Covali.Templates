namespace Covali.Templates.Abstractions.Models;

/// <summary>
/// Request model for updating an existing template.
/// </summary>
public sealed class UpdateTemplateRequest
{
    public required Guid TemplateId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required object Body { get; init; }
}