using Covali.Templates.Abstractions;
using Covali.Templates.EntityFramework;
using Covali.Templates.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Covali.Templates.Extensions;

/// <summary>
/// Extension methods for registering Covali.Templates services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds core template services to the dependency injection container.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: You must also register your DbContext that inherits from AbstractTemplateDbContext.
    /// After registering your DbContext, call services.AddTemplateDbContext&lt;YourDbContext&gt;() to map the interface.
    /// </remarks>
    /// <example>
    /// <code>
    /// services.AddDbContext&lt;TemplatesDbContext&gt;(...);
    /// services.AddTemplateDbContext&lt;TemplatesDbContext&gt;();
    /// services.AddCovaliTemplates();
    /// </code>
    /// </example>
    public static IServiceCollection AddCovaliTemplates(this IServiceCollection services)
    {
        services.AddScoped<ITemplateService, TemplateService>();
        RegisterRepositoriesWithValidation(services);
        return services;
    }

    /// <summary>
    /// Registers the DbContext implementation as ITemplateDbContext.
    /// Call this AFTER registering your DbContext that inherits from AbstractTemplateDbContext.
    /// </summary>
    /// <typeparam name="TDbContext">Your DbContext type that inherits from AbstractTemplateDbContext.</typeparam>
    /// <example>
    /// services.AddDbContext&lt;TemplatesDbContext&gt;(...);
    /// services.AddTemplateDbContext&lt;TemplatesDbContext&gt;();
    /// </example>
    public static IServiceCollection AddTemplateDbContext<TDbContext>(
        this IServiceCollection services
    )
        where TDbContext : AbstractTemplateDbContext
    {
        services.AddScoped<ITemplateDbContext>(sp => sp.GetRequiredService<TDbContext>());
        return services;
    }

    /// <summary>
    /// Validates that ITemplateDbContext is properly registered in the service collection.
    /// Throws InvalidOperationException if the interface is not registered.
    /// Call this after all services are registered to ensure proper configuration.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when ITemplateDbContext is not registered.
    /// </exception>
    public static IServiceCollection ValidateTemplatesConfiguration(this IServiceCollection services)
    {
        var dbContextDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ITemplateDbContext));

        if (dbContextDescriptor == null)
        {
            throw new InvalidOperationException(
                $"ITemplateDbContext is not registered. " +
                $"Please register your DbContext implementation using: " +
                $"services.AddTemplateDbContext<YourDbContext>() " +
                $"or manually register: " +
                $"services.AddScoped<ITemplateDbContext>(sp => sp.GetRequiredService<YourDbContext>());"
            );
        }

        return services;
    }

    /// <summary>
    /// Registers repositories with runtime validation for ITemplateDbContext.
    /// This ensures that if ITemplateDbContext is not registered, a clear exception
    /// is thrown when repositories are first resolved.
    /// </summary>
    private static void RegisterRepositoriesWithValidation(IServiceCollection services)
    {
        services.AddScoped<ITemplateRepository>(sp =>
            {
                var dbContext = sp.GetService<ITemplateDbContext>()
                    ?? throw new InvalidOperationException(
                        $"ITemplateDbContext is not registered. " +
                        $"Repositories require ITemplateDbContext to be available. " +
                        $"Please register your DbContext implementation using: " +
                        $"services.AddTemplateDbContext<YourDbContext>() " +
                        $"BEFORE calling services.AddCovaliTemplates()."
                    );

                return new TemplateRepository(dbContext);
            }
        );
    }
}