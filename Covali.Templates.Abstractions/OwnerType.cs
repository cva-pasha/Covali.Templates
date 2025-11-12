namespace Covali.Templates.Abstractions;

/// <summary>
/// Represents the type of entity that owns a template.
/// </summary>
public enum OwnerType
{
    /// <summary>
    /// Template is owned by an individual user.
    /// </summary>
    User = 1,

    /// <summary>
    /// Template is owned by a group (e.g., family, team).
    /// </summary>
    Group = 2,

    /// <summary>
    /// Template is owned by an organization.
    /// </summary>
    Organization = 3
}