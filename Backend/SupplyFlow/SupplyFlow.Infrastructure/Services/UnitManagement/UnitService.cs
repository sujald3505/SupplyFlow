using Microsoft.EntityFrameworkCore;
using SupplyFlow.Application.DTOs.Unit;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.UnitManagement;

public class UnitService : IUnitService
{
    private readonly SupplyFlowDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UnitService(
        SupplyFlowDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }


    public async Task<List<UnitDto>> GetAllAsync()
    {
        var companyId = GetCompanyId();

        return await _context.Units
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.Name)
            .Select(x => new UnitDto
            {
                Id = x.Id,
                Name = x.Name,
                Symbol = x.Symbol,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }


    public async Task<UnitDto> GetByIdAsync(int id)
    {
        var companyId = GetCompanyId();

        var unit = await _context.Units
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId);

        if (unit == null)
        {
            throw new KeyNotFoundException(
                "Unit not found.");
        }

        return MapToDto(unit);
    }


    public async Task<UnitDto> CreateAsync(
        CreateUnitDto request)
    {
        var companyId = GetCompanyId();

        var unitName = request.Name.Trim();
        var unitSymbol = request.Symbol.Trim();

        var exists = await _context.Units
            .AnyAsync(x =>
                x.CompanyId == companyId &&
                (x.Name == unitName ||
                 x.Symbol == unitSymbol));

        if (exists)
        {
            throw new InvalidOperationException(
                "Unit name or symbol already exists.");
        }

        var unit =
            new SupplyFlow.Domain.Entities.Unit
            {
                CompanyId = companyId,
                Name = unitName,
                Symbol = unitSymbol,
                IsActive = true
            };

        _context.Units.Add(unit);

        await _context.SaveChangesAsync();

        return MapToDto(unit);
    }


    public async Task<UnitDto> UpdateAsync(
        int id,
        UpdateUnitDto request)
    {
        var companyId = GetCompanyId();

        var unit = await _context.Units
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId);

        if (unit == null)
        {
            throw new KeyNotFoundException(
                "Unit not found.");
        }

        var unitName = request.Name.Trim();
        var unitSymbol = request.Symbol.Trim();

        var exists = await _context.Units
            .AnyAsync(x =>
                x.Id != id &&
                x.CompanyId == companyId &&
                (x.Name == unitName ||
                 x.Symbol == unitSymbol));

        if (exists)
        {
            throw new InvalidOperationException(
                "Another unit with this name or symbol already exists.");
        }

        unit.Name = unitName;
        unit.Symbol = unitSymbol;
        unit.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return MapToDto(unit);
    }


    private int GetCompanyId()
    {
        if (!_currentUser.CompanyId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "Company information not found.");
        }

        return _currentUser.CompanyId.Value;
    }


    private static UnitDto MapToDto(
        SupplyFlow.Domain.Entities.Unit unit)
    {
        return new UnitDto
        {
            Id = unit.Id,
            Name = unit.Name,
            Symbol = unit.Symbol,
            IsActive = unit.IsActive
        };
    }
}