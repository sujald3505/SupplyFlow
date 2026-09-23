using Microsoft.EntityFrameworkCore;
using SupplyFlow.Application.DTOs.Company;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.CompanyManagement;

public class CompanyService : ICompanyService
{
    private readonly SupplyFlowDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CompanyService(
        SupplyFlowDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CompanyDto> GetCurrentCompanyAsync()
    {
        var companyId = _currentUser.CompanyId;

        if (!companyId.HasValue)
            throw new UnauthorizedAccessException(
                "Company information not found.");

        var company = await _context.Companies
            .FirstOrDefaultAsync(x =>
                x.Id == companyId.Value);

        if (company == null)
            throw new KeyNotFoundException(
                "Company not found.");

        return MapToDto(company);
    }

    public async Task<CompanyDto> UpdateAsync(
        UpdateCompanyDto request)
    {
        var companyId = _currentUser.CompanyId;

        if (!companyId.HasValue)
            throw new UnauthorizedAccessException(
                "Company information not found.");

        var company = await _context.Companies
            .FirstOrDefaultAsync(x =>
                x.Id == companyId.Value);

        if (company == null)
            throw new KeyNotFoundException(
                "Company not found.");

        company.Name = request.Name.Trim();
        company.Email = request.Email.Trim();
        company.Phone = request.Phone?.Trim();
        company.Address = request.Address?.Trim();
        company.GSTNumber = request.GSTNumber?.Trim();

        await _context.SaveChangesAsync();

        return MapToDto(company);
    }

    private static CompanyDto MapToDto(
        SupplyFlow.Domain.Entities.Company company)
    {
        return new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            Email = company.Email,
            Phone = company.Phone,
            Address = company.Address,
            GSTNumber = company.GSTNumber,
            IsActive = company.IsActive
        };
    }
}