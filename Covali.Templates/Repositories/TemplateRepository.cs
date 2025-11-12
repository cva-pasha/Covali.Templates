using Covali.Templates.Abstractions;
using Covali.Templates.EntityFramework;
using Covali.Templates.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Covali.Templates.Repositories;

/// <summary>
/// Repository implementation for template data access.
/// Internal infrastructure component - works with entities.
/// </summary>
public sealed class TemplateRepository : ITemplateRepository
{
    private readonly ITemplateDbContext _context;

    public TemplateRepository(ITemplateDbContext context)
    {
        _context = context;
    }

    public async Task<Template?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Templates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, ct);
    }

    public async Task<IReadOnlyCollection<Template>> GetByOwnerAsync(
        Guid ownerId,
        OwnerType ownerType,
        string? templateType,
        TemplateSortBy sortBy,
        int page,
        int pageSize,
        CancellationToken ct = default
    )
    {
        var query = _context.Templates
            .AsNoTracking()
            .Where(t => t.OwnerId == ownerId && t.OwnerType == ownerType);

        if (!string.IsNullOrEmpty(templateType))
        {
            query = query.Where(t => t.TemplateType == templateType);
        }

        query = sortBy switch
        {
            TemplateSortBy.UsageCount => query.OrderByDescending(t => t.UsageCount),
            TemplateSortBy.Name => query.OrderBy(t => t.Name),
            TemplateSortBy.CreatedAt => query.OrderByDescending(t => t.CreatedAt),
            TemplateSortBy.UpdatedAt => query.OrderByDescending(t => t.UpdatedAt),
            _ => query.OrderByDescending(t => t.UsageCount)
        };

        var templates = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return templates;
    }

    public async Task<IReadOnlyCollection<Template>> GetMostUsedAsync(
        Guid ownerId,
        OwnerType ownerType,
        string? templateType,
        int limit,
        CancellationToken ct = default
    )
    {
        var query = _context.Templates
            .AsNoTracking()
            .Where(t => t.OwnerId == ownerId && t.OwnerType == ownerType);

        if (!string.IsNullOrEmpty(templateType))
        {
            query = query.Where(t => t.TemplateType == templateType);
        }

        var templates = await query
            .OrderByDescending(t => t.UsageCount)
            .Take(limit)
            .ToListAsync(ct);

        return templates;
    }

    public async Task<Template> AddAsync(Template template, CancellationToken ct = default)
    {
        _context.Templates.Add(template);
        await _context.SaveChangesAsync(ct);
        return template;
    }

    public async Task<Template> UpdateAsync(Template template, CancellationToken ct = default)
    {
        _context.Templates.Update(template);
        await _context.SaveChangesAsync(ct);
        return template;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var template = await _context.Templates.FindAsync([id], ct);
        if (template is null)
        {
            return false;
        }

        _context.Templates.Remove(template);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<int> IncrementUsageAsync(Guid id, CancellationToken ct = default)
    {
        var template = await _context.Templates.FindAsync([id], ct);
        if (template is null)
        {
            throw new InvalidOperationException($"Template with ID {id} not found.");
        }

        template.UsageCount++;
        await _context.SaveChangesAsync(ct);
        return template.UsageCount;
    }

    public async Task<bool> ExistsAsync(
        Guid ownerId,
        OwnerType ownerType,
        string templateType,
        string name,
        CancellationToken ct = default
    )
    {
        return await _context.Templates
            .AsNoTracking()
            .AnyAsync(
                t => t.OwnerId == ownerId
                    && t.OwnerType == ownerType
                    && t.TemplateType == templateType
                    && t.Name == name,
                ct
            );
    }
}