using SupplyFlow.Application.DTOs.Warehouse;

namespace SupplyFlow.Application.Interfaces.Services;

public interface IWarehouseService
{
    Task<List<WarehouseDto>> GetAllAsync();

    Task<WarehouseDto> GetByIdAsync(int id);

    Task<WarehouseDto> CreateAsync(
        CreateWarehouseDto request);

    Task<WarehouseDto> UpdateAsync(
        int id,
        UpdateWarehouseDto request);
}