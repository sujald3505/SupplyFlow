using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplyFlow.Domain.Entities;

namespace SupplyFlow.Infrastructure.Configurations;

public class WarehouseStockConfiguration
    : IEntityTypeConfiguration<WarehouseStock>
{
    public void Configure(EntityTypeBuilder<WarehouseStock> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 2);

        builder.HasIndex(x => new
        {
            x.CompanyId,
            x.WarehouseId,
            x.ProductId
        }).IsUnique();

        builder
            .HasOne(x => x.Company)
            .WithMany(x => x.WarehouseStocks)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Warehouse)
            .WithMany(x => x.WarehouseStocks)
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.Product)
            .WithMany(x => x.WarehouseStocks)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}