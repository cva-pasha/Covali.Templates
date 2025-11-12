using System.Text.Json;
using Covali.Templates.Abstractions;
using Covali.Templates.Abstractions.Models;
using Covali.Templates.EntityFramework.Entities;
using Covali.Templates.Repositories;

namespace Covali.Templates;

/// <summary>
/// Service implementation for managing templates.
/// Works with DTOs and provides business logic.
/// </summary>
public sealed class TemplateService : ITemplateService
{
    private readonly ITemplateRepository _repository;

    public TemplateService(ITemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<TemplateDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var template = await _repository.GetByIdAsync(id, ct);
        return template is null ? null : MapToDto(template);
    }

    public async Task<IReadOnlyCollection<TemplateDto>> GetByOwnerAsync(
        Guid ownerId,
        OwnerType ownerType,
        string? templateType,
        TemplateSortBy sortBy,
        int page,
        int pageSize,
        CancellationToken ct = default
    )
    {
        var templates = await _repository.GetByOwnerAsync(ownerId, ownerType, templateType, sortBy, page, pageSize, ct);
        return templates.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyCollection<TemplateDto>> GetMostUsedAsync(
        Guid ownerId,
        OwnerType ownerType,
        string? templateType,
        int limit,
        CancellationToken ct = default
    )
    {
        var templates = await _repository.GetMostUsedAsync(ownerId, ownerType, templateType, limit, ct);
        return templates.Select(MapToDto).ToList();
    }

    public async Task<TemplateDto> AddAsync(CreateTemplateRequest request, CancellationToken ct = default)
    {
        var exists = await _repository.ExistsAsync(request.OwnerId, request.OwnerType, request.TemplateType, request.Name, ct);
        if (exists)
        {
            throw new InvalidOperationException($"Template with name '{request.Name}' already exists for this owner.");
        }

        var bodyJson = JsonSerializer.Serialize(request.Body);

        if (bodyJson.Length > Template.MaxBodySize)
        {
            throw new InvalidOperationException($"Template body exceeds maximum size of {Template.MaxBodySize} bytes.");
        }

        var template = new Template
        {
            Id = Guid.NewGuid(),
            OwnerId = request.OwnerId,
            OwnerType = request.OwnerType,
            TemplateType = request.TemplateType,
            Name = request.Name,
            Description = request.Description,
            Body = bodyJson,
            UsageCount = 0,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _repository.AddAsync(template, ct);
        return MapToDto(template);
    }

    public async Task<TemplateDto> UpdateAsync(Guid id, UpdateTemplateRequest request, CancellationToken ct = default)
    {
        var template = await _repository.GetByIdAsync(id, ct);
        if (template is null)
        {
            throw new InvalidOperationException($"Template with ID {id} not found.");
        }

        var bodyJson = JsonSerializer.Serialize(request.Body);

        if (bodyJson.Length > Template.MaxBodySize)
        {
            throw new InvalidOperationException($"Template body exceeds maximum size of {Template.MaxBodySize} bytes.");
        }

        template.Name = request.Name;
        template.Description = request.Description;
        template.Body = bodyJson;
        template.UpdatedAt = DateTimeOffset.UtcNow;

        await _repository.UpdateAsync(template, ct);
        return MapToDto(template);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        return await _repository.DeleteAsync(id, ct);
    }

    public async Task<int> IncrementUsageAsync(Guid id, CancellationToken ct = default)
    {
        return await _repository.IncrementUsageAsync(id, ct);
    }

    public async Task<bool> ExistsAsync(
        Guid ownerId,
        OwnerType ownerType,
        string templateType,
        string name,
        CancellationToken ct = default
    )
    {
        return await _repository.ExistsAsync(ownerId, ownerType, templateType, name, ct);
    }

    private static TemplateDto MapToDto(Template template)
    {
        var body = JsonSerializer.Deserialize<object>(template.Body) ?? new object();

        return new TemplateDto
        {
            Id = template.Id,
            OwnerId = template.OwnerId,
            OwnerType = template.OwnerType,
            TemplateType = template.TemplateType,
            Name = template.Name,
            Description = template.Description,
            Body = body,
            UsageCount = template.UsageCount,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt
        };
    }
}