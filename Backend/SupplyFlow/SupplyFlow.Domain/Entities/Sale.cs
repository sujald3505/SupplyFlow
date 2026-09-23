using SupplyFlow.Domain.Common;

namespace SupplyFlow.Domain.Entities;

public class Sale : BaseEntity
{
    public int CompanyId { get; set; }

    public int WarehouseId { get; set; }

    public int CustomerId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty;

    public DateTime SaleDate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Discount { get; set; }

    public decimal Tax { get; set; }

    public decimal GrandTotal { get; set; }

    public string Status { get; set; } = "Completed";

    public string? Remarks { get; set; }

    

    public Customer Customer { get; set; } = null!;

    // Navigation Properties
    public Company Company { get; set; } = null!;

    public Warehouse Warehouse { get; set; } = null!;

    public ICollection<SaleItem> Items { get; set; }
        = new List<SaleItem>();
}