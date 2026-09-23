using SupplyFlow.Application.DTOs.WarehouseStock;

namespace SupplyFlow.Application.Interfaces.Services;

public interface IWarehouseStockService
{
    Task<List<WarehouseStockDto>> GetAllAsync();

    Task<List<WarehouseStockDto>>
        GetByWarehouseIdAsync(int warehouseId);

    Task<List<WarehouseStockDto>>
        GetByProductIdAsync(int productId);

    Task<List<LowStockDto>>
        GetLowStockAsync();
}