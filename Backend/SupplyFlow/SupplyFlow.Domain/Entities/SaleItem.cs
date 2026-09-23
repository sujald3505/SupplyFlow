using SupplyFlow.Domain.Common;

namespace SupplyFlow.Domain.Entities;

public class SaleItem : BaseEntity
{
    public int SaleId { get; set; }

    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }

    // Navigation Properties
    public Sale Sale { get; set; } = null!;

    public Product Product { get; set; } = null!;
}