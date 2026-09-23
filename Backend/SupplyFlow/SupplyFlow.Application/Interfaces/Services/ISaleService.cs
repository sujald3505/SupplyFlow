using SupplyFlow.Application.DTOs.Sale;

namespace SupplyFlow.Application.Interfaces.Services;

public interface ISaleService
{
    Task<List<SaleDto>> GetAllAsync();

    Task<SaleDto?> GetByIdAsync(int id);

    Task<SaleDto> CreateAsync(CreateSaleDto request);
}