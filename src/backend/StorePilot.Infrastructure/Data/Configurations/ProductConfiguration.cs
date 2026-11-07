using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorePilot.Domain.Entities;

namespace StorePilot.Infrastructure.Data.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Code)
            .HasMaxLength(20);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.CurrentQuantity)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(p => p.MinQuantity)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(p => p.SalePrice)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(p => p.PurchasePrice)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(p => p.UnitId)
            .IsRequired();

        builder.Property(p => p.GroupId)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(p => p.Notes)
            .HasMaxLength(100);

        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(p => p.Name)
            .IsUnique()
            .HasDatabaseName("UQ_Products_Name");

        builder.HasIndex(p => p.Code)
            .IsUnique()
            .HasDatabaseName("UQ_Products_Code");
    }
}
