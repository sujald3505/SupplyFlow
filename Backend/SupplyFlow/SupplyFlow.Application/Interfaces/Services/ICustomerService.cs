using SupplyFlow.Application.DTOs.Customer;

namespace SupplyFlow.Application.Interfaces.Services;

public interface ICustomerService
{
    Task<List<CustomerDto>> GetAllAsync();

    Task<CustomerDto?> GetByIdAsync(int id);

    Task<CustomerDto> CreateAsync(
        CreateCustomerDto request);
}