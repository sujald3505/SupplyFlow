using SupplyFlow.Domain.Common;

namespace SupplyFlow.Domain.Entities;

public class WarehouseStock : BaseEntity
{
    public int CompanyId { get; set; }

    public int WarehouseId { get; set; }

    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    // Navigation Properties
    public Company Company { get; set; } = null!;

    public Warehouse Warehouse { get; set; } = null!;

    public Product Product { get; set; } = null!;
}