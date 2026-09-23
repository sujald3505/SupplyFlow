using SupplyFlow.Application.DTOs.SalesReturn;

namespace SupplyFlow.Application.Interfaces.Services;

public interface ISalesReturnService
{
    Task<List<SalesReturnDto>> GetAllAsync();

    Task<SalesReturnDto?> GetByIdAsync(int id);

    Task<SalesReturnDto> CreateAsync(
        CreateSalesReturnDto request);
}