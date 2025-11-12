using Covali.Templates.Abstractions;
using Covali.Templates.EntityFramework.Entities;

namespace Covali.Templates.Repositories;

/// <summary>
/// Repository interface for template data access.
/// Works with entities internally (infrastructure concern).
/// </summary>
public interface ITemplateRepository
{
    Task<Template?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyCollection<Template>> GetByOwnerAsync(
        Guid ownerId,
        OwnerType ownerType,
        string? templateType,
        TemplateSortBy sortBy,
        int page,
        int pageSize,
        CancellationToken ct = default
    );

    Task<IReadOnlyCollection<Template>> GetMostUsedAsync(
        Guid ownerId,
        OwnerType ownerType,
        string? templateType,
        int limit,
        CancellationToken ct = default
    );

    Task<Template> AddAsync(Template template, CancellationToken ct = default);
    Task<Template> UpdateAsync(Template template, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<int> IncrementUsageAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid ownerId, OwnerType ownerType, string templateType, string name, CancellationToken ct = default);
}