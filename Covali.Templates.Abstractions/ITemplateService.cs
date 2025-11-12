using Covali.Templates.Abstractions.Models;

namespace Covali.Templates.Abstractions;

/// <summary>
/// Service interface for managing templates at a high level.
/// Works with DTOs and provides business logic operations.
/// </summary>
public interface ITemplateService
{
    Task<TemplateDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyCollection<TemplateDto>> GetByOwnerAsync(
        Guid ownerId,
        OwnerType ownerType,
        string? templateType,
        TemplateSortBy sortBy,
        int page,
        int pageSize,
        CancellationToken ct = default
    );

    Task<IReadOnlyCollection<TemplateDto>> GetMostUsedAsync(
        Guid ownerId,
        OwnerType ownerType,
        string? templateType,
        int limit,
        CancellationToken ct = default
    );

    Task<TemplateDto> AddAsync(CreateTemplateRequest request, CancellationToken ct = default);
    Task<TemplateDto> UpdateAsync(Guid id, UpdateTemplateRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<int> IncrementUsageAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid ownerId, OwnerType ownerType, string templateType, string name, CancellationToken ct = default);
}