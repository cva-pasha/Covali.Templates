namespace Covali.Templates.Abstractions.Models;

/// <summary>
/// Request model for retrieving templates.
/// </summary>
public sealed class GetTemplatesRequest
{
    public required Guid OwnerId { get; init; }
    public required OwnerType OwnerType { get; init; }
    public string? TemplateType { get; init; }
    public TemplateSortBy SortBy { get; init; } = TemplateSortBy.UsageCount;
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}