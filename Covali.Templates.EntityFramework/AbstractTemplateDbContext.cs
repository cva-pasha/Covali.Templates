using Covali.Templates.EntityFramework.Configurations;
using Covali.Templates.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Covali.Templates.EntityFramework;

/// <summary>
/// Abstract base class for template database contexts.
/// Provides core template entities and configurations.
/// Projects should inherit from this class and provide their own schema.
/// </summary>
/// <remarks>
/// Example usage:
/// <code>
/// public class MyTemplatesDbContext : AbstractTemplateDbContext
/// {
///     public MyTemplatesDbContext(DbContextOptions options) : base(options, "my_schema") { }
/// }
/// </code>
/// </remarks>
public abstract class AbstractTemplateDbContext : DbContext, ITemplateDbContext
{
    private readonly string _schema;

    /// <summary>
    /// Initializes a new instance of the AbstractTemplateDbContext.
    /// </summary>
    /// <param name="options">The database context options.</param>
    /// <param name="schema">The database schema to use for template tables.</param>
    protected AbstractTemplateDbContext(DbContextOptions options, string schema)
        : base(options)
    {
        if (string.IsNullOrWhiteSpace(schema))
        {
            schema = "CovaliTemplates";
        }

        _schema = schema;
    }

    /// <summary>
    /// Template instances for storing reusable data structures.
    /// </summary>
    public DbSet<Template> Templates => Set<Template>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ApplyDefaultConfigurations(modelBuilder);
    }

    protected void ApplyDefaultConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TemplateConfiguration(_schema));
    }
}