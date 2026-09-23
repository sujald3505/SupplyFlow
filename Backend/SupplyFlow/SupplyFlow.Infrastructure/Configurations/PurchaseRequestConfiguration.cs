using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupplyFlow.Domain.Entities;

namespace SupplyFlow.Infrastructure.Configurations;

public class PurchaseRequestConfiguration
    : IEntityTypeConfiguration<PurchaseRequest>
{
    public void Configure(EntityTypeBuilder<PurchaseRequest> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RequestNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.CompanyId,
            x.RequestNumber
        }).IsUnique();

        builder.Property(x => x.Remarks)
            .HasMaxLength(500);

        builder.Property(x => x.RejectionReason)
            .HasMaxLength(500);

        // Company → PurchaseRequests relationship
        builder
            .HasOne(x => x.Company)
            .WithMany(x => x.PurchaseRequests)
            .HasForeignKey(x => x.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}