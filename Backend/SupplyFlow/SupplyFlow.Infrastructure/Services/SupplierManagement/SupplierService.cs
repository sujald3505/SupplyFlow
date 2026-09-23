using Microsoft.EntityFrameworkCore;
using SupplyFlow.Application.DTOs.Supplier;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.SupplierManagement;

public class SupplierService : ISupplierService
{
    private readonly SupplyFlowDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public SupplierService(
        SupplyFlowDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }


    public async Task<List<SupplierDto>> GetAllAsync()
    {
        var companyId = GetCompanyId();

        return await _context.Suppliers
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.Name)
            .Select(x => new SupplierDto
            {
                Id = x.Id,
                Name = x.Name,
                ContactPerson = x.ContactPerson,
                Email = x.Email,
                Phone = x.Phone,
                Address = x.Address,
                GSTNumber = x.GSTNumber,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }


    public async Task<SupplierDto> GetByIdAsync(
        int id)
    {
        var companyId = GetCompanyId();

        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId);

        if (supplier == null)
        {
            throw new KeyNotFoundException(
                "Supplier not found.");
        }

        return MapToDto(supplier);
    }


    public async Task<SupplierDto> CreateAsync(
        CreateSupplierDto request)
    {
        var companyId = GetCompanyId();

        var name = request.Name.Trim();

        var nameExists = await _context.Suppliers
            .AnyAsync(x =>
                x.CompanyId == companyId &&
                x.Name == name);

        if (nameExists)
        {
            throw new InvalidOperationException(
                "Supplier name already exists.");
        }

        // GST number uniqueness check
        var gstNumber = request.GSTNumber?.Trim();

        if (!string.IsNullOrWhiteSpace(gstNumber))
        {
            var gstExists = await _context.Suppliers
                .AnyAsync(x =>
                    x.CompanyId == companyId &&
                    x.GSTNumber == gstNumber);

            if (gstExists)
            {
                throw new InvalidOperationException(
                    "Supplier GST number already exists.");
            }
        }

        var supplier =
            new SupplyFlow.Domain.Entities.Supplier
            {
                CompanyId = companyId,
                Name = name,
                ContactPerson =
                    request.ContactPerson.Trim(),
                Email = request.Email?.Trim(),
                Phone = request.Phone?.Trim(),
                Address = request.Address?.Trim(),
                GSTNumber = gstNumber,
                IsActive = true
            };

        _context.Suppliers.Add(supplier);

        await _context.SaveChangesAsync();

        return MapToDto(supplier);
    }


    public async Task<SupplierDto> UpdateAsync(
        int id,
        UpdateSupplierDto request)
    {
        var companyId = GetCompanyId();

        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId);

        if (supplier == null)
        {
            throw new KeyNotFoundException(
                "Supplier not found.");
        }

        var name = request.Name.Trim();

        var nameExists = await _context.Suppliers
            .AnyAsync(x =>
                x.Id != id &&
                x.CompanyId == companyId &&
                x.Name == name);

        if (nameExists)
        {
            throw new InvalidOperationException(
                "Another supplier with this name already exists.");
        }

        var gstNumber = request.GSTNumber?.Trim();

        if (!string.IsNullOrWhiteSpace(gstNumber))
        {
            var gstExists = await _context.Suppliers
                .AnyAsync(x =>
                    x.Id != id &&
                    x.CompanyId == companyId &&
                    x.GSTNumber == gstNumber);

            if (gstExists)
            {
                throw new InvalidOperationException(
                    "Another supplier with this GST number already exists.");
            }
        }

        supplier.Name = name;
        supplier.ContactPerson =
            request.ContactPerson.Trim();

        supplier.Email =
            request.Email?.Trim();

        supplier.Phone =
            request.Phone?.Trim();

        supplier.Address =
            request.Address?.Trim();

        supplier.GSTNumber = gstNumber;

        supplier.IsActive =
            request.IsActive;

        await _context.SaveChangesAsync();

        return MapToDto(supplier);
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


    private static SupplierDto MapToDto(
        SupplyFlow.Domain.Entities.Supplier supplier)
    {
        return new SupplierDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            ContactPerson = supplier.ContactPerson,
            Email = supplier.Email,
            Phone = supplier.Phone,
            Address = supplier.Address,
            GSTNumber = supplier.GSTNumber,
            IsActive = supplier.IsActive
        };
    }
}