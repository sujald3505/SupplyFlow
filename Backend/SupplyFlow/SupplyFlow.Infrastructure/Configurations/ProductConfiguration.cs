//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using SupplyFlow.Domain.Entities;

//namespace SupplyFlow.Infrastructure.Configurations;

//public class ProductConfiguration : IEntityTypeConfiguration<Product>
//{
//    public void Configure(EntityTypeBuilder<Product> builder)
//    {
//        builder.HasKey(x => x.Id);

//        builder.Property(x => x.Name)
//            .HasMaxLength(200)
//            .IsRequired();

//        builder.Property(x => x.SKU)
//            .HasMaxLength(100)
//            .IsRequired();

//        builder.Property(x => x.Description)
//            .HasMaxLength(1000);

//        builder.Property(x => x.CostPrice)
//            .HasPrecision(18, 2);

//        builder.Property(x => x.SellingPrice)
//            .HasPrecision(18, 2);

//        builder.Property(x => x.MinimumStockLevel)
//            .HasPrecision(18, 2);

//        builder.HasIndex(x => new
//        {
//            x.CompanyId,
//            x.SKU
//        }).IsUnique();

//        builder
//            .HasOne(x => x.Company)
//            .WithMany(x => x.Products)
//            .HasForeignKey(x => x.CompanyId)
//            .OnDelete(DeleteBehavior.Restrict);

//        builder
//            .HasOne(x => x.Category)
//            .WithMany(x => x.Products)
//            .HasForeignKey(x => x.CategoryId)
//            .OnDelete(DeleteBehavior.Restrict);

//        builder
//            .HasOne(x => x.Unit)
//            .WithMany(x => x.Products)
//            .HasForeignKey(x => x.UnitId)
//            .OnDelete(DeleteBehavior.Restrict);
//    }
//}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplyFlow.Domain.Entities;

namespace SupplyFlow.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.SKU)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500);

        builder.Property(x => x.CostPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.SellingPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.MinimumStockLevel)
            .HasPrecision(18, 2);

        builder.HasIndex(x => new
        {
            x.CompanyId,
            x.SKU
        }).IsUnique();

        builder
            .HasOne(x => x.Company)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Unit)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.UnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}