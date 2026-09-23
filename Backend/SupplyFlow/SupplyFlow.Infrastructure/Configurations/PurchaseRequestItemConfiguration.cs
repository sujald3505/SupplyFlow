using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplyFlow.Domain.Entities;

namespace SupplyFlow.Infrastructure.Configurations;

public class PurchaseRequestItemConfiguration
    : IEntityTypeConfiguration<PurchaseRequestItem>
{
    public void Configure(
        EntityTypeBuilder<PurchaseRequestItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RequestedQuantity)
            .HasPrecision(18, 2);

        builder.Property(x => x.Remarks)
            .HasMaxLength(500);

        builder
            .HasOne(x => x.PurchaseRequest)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.PurchaseRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Product)
            .WithMany(x => x.PurchaseRequestItems)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}