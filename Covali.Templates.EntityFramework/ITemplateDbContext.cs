using Covali.Templates.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Covali.Templates.EntityFramework;

/// <summary>
/// Interface for template database context.
/// This interface is used for dependency injection to decouple repositories from concrete DbContext implementations.
/// </summary>
public interface ITemplateDbContext
{
    /// <summary>
    /// Template instances for storing reusable data structures.
    /// </summary>
    DbSet<Template> Templates { get; }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}