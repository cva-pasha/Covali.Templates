namespace Covali.Templates.Abstractions;

/// <summary>
/// Sorting options for template queries.
/// </summary>
public enum TemplateSortBy
{
    /// <summary>
    /// Sort by usage count (descending).
    /// </summary>
    UsageCount = 1,

    /// <summary>
    /// Sort by template name (ascending).
    /// </summary>
    Name = 2,

    /// <summary>
    /// Sort by creation date (descending).
    /// </summary>
    CreatedAt = 3,

    /// <summary>
    /// Sort by last update date (descending).
    /// </summary>
    UpdatedAt = 4
}