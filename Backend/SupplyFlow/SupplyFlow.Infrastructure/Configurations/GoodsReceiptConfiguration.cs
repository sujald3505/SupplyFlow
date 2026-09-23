using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplyFlow.Domain.Entities;

namespace SupplyFlow.Infrastructure.Configurations;

public class GoodsReceiptConfiguration
    : IEntityTypeConfiguration<GoodsReceipt>
{
    public void Configure(EntityTypeBuilder<GoodsReceipt> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.GRNNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.CompanyId,
            x.GRNNumber
        }).IsUnique();

        builder.Property(x => x.Remarks)
            .HasMaxLength(500);
    }
}