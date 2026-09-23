namespace SupplyFlow.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    // Products
    public int TotalProducts { get; set; }

    public int ActiveProducts { get; set; }

    // Warehouses
    public int TotalWarehouses { get; set; }

    public int ActiveWarehouses { get; set; }

    // Suppliers
    public int TotalSuppliers { get; set; }

    public int ActiveSuppliers { get; set; }

    // Inventory
    public decimal TotalStockQuantity { get; set; }

    public decimal TotalInventoryValue { get; set; }

    // Low Stock
    public int LowStockProducts { get; set; }

    // Purchase Requests
    public int DraftPurchaseRequests { get; set; }

    public int SubmittedPurchaseRequests { get; set; }

    // Purchase Orders
    public int DraftPurchaseOrders { get; set; }

    public int ConfirmedPurchaseOrders { get; set; }

    // Goods Receipts
    public int CompletedGoodsReceipts { get; set; }
}