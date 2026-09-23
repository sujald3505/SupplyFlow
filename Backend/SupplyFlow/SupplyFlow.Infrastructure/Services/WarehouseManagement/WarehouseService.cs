using Microsoft.EntityFrameworkCore;
using SupplyFlow.Application.DTOs.Warehouse;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.WarehouseManagement;

public class WarehouseService : IWarehouseService
{
    private readonly SupplyFlowDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public WarehouseService(
        SupplyFlowDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }


    public async Task<List<WarehouseDto>> GetAllAsync()
    {
        var companyId = GetCompanyId();

        return await _context.Warehouses
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.Name)
            .Select(x => new WarehouseDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Address = x.Address,
                ContactPerson = x.ContactPerson,
                Phone = x.Phone,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }


    public async Task<WarehouseDto> GetByIdAsync(
        int id)
    {
        var companyId = GetCompanyId();

        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId);

        if (warehouse == null)
        {
            throw new KeyNotFoundException(
                "Warehouse not found.");
        }

        return MapToDto(warehouse);
    }


    public async Task<WarehouseDto> CreateAsync(
        CreateWarehouseDto request)
    {
        var companyId = GetCompanyId();

        var name = request.Name.Trim();
        var code = request.Code?.Trim();

        // Check duplicate warehouse name
        var nameExists = await _context.Warehouses
            .AnyAsync(x =>
                x.CompanyId == companyId &&
                x.Name == name);

        if (nameExists)
        {
            throw new InvalidOperationException(
                "Warehouse name already exists.");
        }

        // Check duplicate code only if code is provided
        if (!string.IsNullOrWhiteSpace(code))
        {
            var codeExists = await _context.Warehouses
                .AnyAsync(x =>
                    x.CompanyId == companyId &&
                    x.Code == code);

            if (codeExists)
            {
                throw new InvalidOperationException(
                    "Warehouse code already exists.");
            }
        }

        var warehouse =
            new SupplyFlow.Domain.Entities.Warehouse
            {
                CompanyId = companyId,
                Name = name,
                Code = code,
                Address = request.Address?.Trim(),
                ContactPerson =
                    request.ContactPerson?.Trim(),
                Phone = request.Phone?.Trim(),
                IsActive = true
            };

        _context.Warehouses.Add(warehouse);

        await _context.SaveChangesAsync();

        return MapToDto(warehouse);
    }


    public async Task<WarehouseDto> UpdateAsync(
        int id,
        UpdateWarehouseDto request)
    {
        var companyId = GetCompanyId();

        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId);

        if (warehouse == null)
        {
            throw new KeyNotFoundException(
                "Warehouse not found.");
        }

        var name = request.Name.Trim();
        var code = request.Code?.Trim();

        // Duplicate name check
        var nameExists = await _context.Warehouses
            .AnyAsync(x =>
                x.Id != id &&
                x.CompanyId == companyId &&
                x.Name == name);

        if (nameExists)
        {
            throw new InvalidOperationException(
                "Another warehouse with this name already exists.");
        }

        // Duplicate code check
        if (!string.IsNullOrWhiteSpace(code))
        {
            var codeExists = await _context.Warehouses
                .AnyAsync(x =>
                    x.Id != id &&
                    x.CompanyId == companyId &&
                    x.Code == code);

            if (codeExists)
            {
                throw new InvalidOperationException(
                    "Another warehouse with this code already exists.");
            }
        }

        warehouse.Name = name;
        warehouse.Code = code;
        warehouse.Address = request.Address?.Trim();
        warehouse.ContactPerson =
            request.ContactPerson?.Trim();
        warehouse.Phone =
            request.Phone?.Trim();

        warehouse.IsActive =
            request.IsActive;

        await _context.SaveChangesAsync();

        return MapToDto(warehouse);
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


    private static WarehouseDto MapToDto(
        SupplyFlow.Domain.Entities.Warehouse warehouse)
    {
        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Code = warehouse.Code,
            Address = warehouse.Address,
            ContactPerson = warehouse.ContactPerson,
            Phone = warehouse.Phone,
            IsActive = warehouse.IsActive
        };
    }
}