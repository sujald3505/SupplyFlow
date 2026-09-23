using SupplyFlow.Application.DTOs.InventoryTransaction;

namespace SupplyFlow.Application.Interfaces.Services;

public interface IInventoryTransactionService
{
    Task<List<InventoryTransactionDto>> GetAllAsync();

    Task<List<InventoryTransactionDto>>
        GetByWarehouseIdAsync(int warehouseId);

    Task<List<InventoryTransactionDto>>
        GetByProductIdAsync(int productId);

    Task<InventoryTransactionDto> CreateAsync(
        CreateInventoryTransactionDto request);
}