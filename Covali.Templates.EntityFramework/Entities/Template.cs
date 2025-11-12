using Covali.EntityFramework;
using Covali.Templates.Abstractions;

namespace Covali.Templates.EntityFramework.Entities;

/// <summary>
/// Template entity for storing reusable data structures.
/// </summary>
public sealed class Template : IHasTimestamps
{
    public const int OwnerTypeMaxLength = 50;
    public const int TemplateTypeMaxLength = 50;
    public const int NameMaxLength = 100;
    public const int DescriptionMaxLength = 500;
    public const int MaxBodySize = 1_048_576; // 1 MB

    public Guid Id { get; init; } = Guid.NewGuid();
    public required Guid OwnerId { get; init; }
    public required OwnerType OwnerType { get; init; }
    public required string TemplateType { get; init; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Body { get; set; }
    public int UsageCount { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}