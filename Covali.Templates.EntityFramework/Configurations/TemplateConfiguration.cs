using Covali.EntityFramework.ValueConverters;
using Covali.Templates.Abstractions;
using Covali.Templates.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Covali.Templates.EntityFramework.Configurations;

/// <summary>
/// Entity Framework configuration for the Template entity.
/// </summary>
public sealed class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
    private readonly string _schema;

    public TemplateConfiguration(string schema)
    {
        _schema = schema;
    }

    public void Configure(EntityTypeBuilder<Template> builder)
    {
        builder.ToTable(name: "Templates", _schema);

        builder.HasKey(t => t.Id);

        builder.HasIndex(t => new { t.OwnerId, t.OwnerType, t.TemplateType });

        builder.HasIndex(t => new { t.OwnerId, t.UsageCount })
            .IsDescending(false, true);

        builder.HasIndex(t => new { t.OwnerId, t.OwnerType, t.TemplateType, t.Name })
            .IsUnique();

        builder.Property(t => t.OwnerId)
            .IsRequired();

        builder.Property(t => t.OwnerType)
            .IsRequired()
            .HasConversion<EnumToStringConverter<OwnerType>>();

        builder.Property(t => t.TemplateType)
            .IsRequired()
            .HasMaxLength(Template.TemplateTypeMaxLength);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(Template.NameMaxLength);

        builder.Property(t => t.Description)
            .HasMaxLength(Template.DescriptionMaxLength)
            .IsRequired(false);

        builder.Property(t => t.Body)
            .IsRequired()
            .HasColumnType("bytea")
            .HasConversion(new CompressedTextConverter());

        builder.Property(t => t.UsageCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired(false);

        builder.Property(t => t.DeletedAt)
            .IsRequired(false);
    }
}