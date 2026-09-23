using SupplyFlow.Domain.Common;

namespace SupplyFlow.Domain.Entities;

public class SalesReturn : BaseEntity
{
    public int CompanyId { get; set; }

    public int SaleId { get; set; }

    public int WarehouseId { get; set; }

    public int CustomerId { get; set; }

    public string ReturnNumber { get; set; } = string.Empty;

    public DateTime ReturnDate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal GrandTotal { get; set; }

    public string Status { get; set; } = "Completed";

    public string? Remarks { get; set; }


    // Navigation Properties

    public Company Company { get; set; } = null!;

    public Sale Sale { get; set; } = null!;

    public Warehouse Warehouse { get; set; } = null!;

    public Customer Customer { get; set; } = null!;

    public ICollection<SalesReturnItem> Items { get; set; }
        = new List<SalesReturnItem>();
}