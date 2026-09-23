using SupplyFlow.Application.DTOs.Company;

namespace SupplyFlow.Application.Interfaces.Services;

public interface ICompanyService
{
    Task<CompanyDto> GetCurrentCompanyAsync();

    Task<CompanyDto> UpdateAsync(
        UpdateCompanyDto request);
}