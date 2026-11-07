using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorePilot.Domain.Entities;

namespace StorePilot.Infrastructure.Data.Configurations;

internal sealed class ProductGroupConfiguration : IEntityTypeConfiguration<ProductGroup>
{
    public void Configure(EntityTypeBuilder<ProductGroup> builder)
    {
        builder.ToTable("Groups");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .ValueGeneratedOnAdd();

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(g => g.Name)
            .IsUnique()
            .HasDatabaseName("UQ_Groups_Name");

        builder.Property(g => g.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(g => g.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(g => g.Notes)
            .HasMaxLength(100);

        builder.HasMany(g => g.Products)
            .WithOne(p => p.Group)
            .HasForeignKey(p => p.GroupId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
