using SupplyFlow.Application.DTOs.Supplier;

namespace SupplyFlow.Application.Interfaces.Services;

public interface ISupplierService
{
    Task<List<SupplierDto>> GetAllAsync();

    Task<SupplierDto> GetByIdAsync(int id);

    Task<SupplierDto> CreateAsync(
        CreateSupplierDto request);

    Task<SupplierDto> UpdateAsync(
        int id,
        UpdateSupplierDto request);
}