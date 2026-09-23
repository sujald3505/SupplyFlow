using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplyFlow.Domain.Entities;

namespace SupplyFlow.Infrastructure.Configurations;

public class GoodsReceiptItemConfiguration
    : IEntityTypeConfiguration<GoodsReceiptItem>
{
    public void Configure(
        EntityTypeBuilder<GoodsReceiptItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReceivedQuantity)
            .HasPrecision(18, 2);

        builder.Property(x => x.AcceptedQuantity)
            .HasPrecision(18, 2);

        builder.Property(x => x.RejectedQuantity)
            .HasPrecision(18, 2);

        builder.Property(x => x.Remarks)
            .HasMaxLength(500);

        builder
            .HasOne(x => x.GoodsReceipt)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.GoodsReceiptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.Product)
            .WithMany(x => x.GoodsReceiptItems)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(x => x.PurchaseOrderItem)
            .WithMany(x => x.GoodsReceiptItems)
            .HasForeignKey(x => x.PurchaseOrderItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}