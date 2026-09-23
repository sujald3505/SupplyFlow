using SupplyFlow.Domain.Common;

namespace SupplyFlow.Domain.Entities;

public class SalesReturnItem : BaseEntity
{
    public int SalesReturnId { get; set; }

    public int SaleItemId { get; set; }

    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }


    // Navigation Properties

    public SalesReturn SalesReturn { get; set; } = null!;

    public SaleItem SaleItem { get; set; } = null!;

    public Product Product { get; set; } = null!;
}