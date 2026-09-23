using Microsoft.EntityFrameworkCore;
using SupplyFlow.Application.DTOs.Customer;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.Customer;

public class CustomerService : ICustomerService
{
    private readonly SupplyFlowDbContext _context;

    public CustomerService(
        SupplyFlowDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerDto>> GetAllAsync()
    {
        return await _context.Customers
            .OrderBy(x => x.Name)
            .Select(x => new CustomerDto
            {
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                Email = x.Email,
                Address = x.Address,
                GSTIN = x.GSTIN,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        return await _context.Customers
            .Where(x => x.Id == id)
            .Select(x => new CustomerDto
            {
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone,
                Email = x.Email,
                Address = x.Address,
                GSTIN = x.GSTIN,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CustomerDto> CreateAsync(
        CreateCustomerDto request)
    {
        var name = request.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException(
                "Customer name is required.");
        }

        var customerExists = await _context.Customers
            .AnyAsync(x =>
                x.Name.ToLower() == name.ToLower());

        if (customerExists)
        {
            throw new InvalidOperationException(
                "Customer already exists.");
        }

        var customer = new SupplyFlow.Domain.Entities.Customer
        {
            CompanyId = 1,
            Name = name,
            Phone = request.Phone?.Trim(),
            Email = request.Email?.Trim(),
            Address = request.Address?.Trim(),
            GSTIN = request.GSTIN?.Trim(),
            IsActive = true
        };

        _context.Customers.Add(customer);

        await _context.SaveChangesAsync();

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Phone = customer.Phone,
            Email = customer.Email,
            Address = customer.Address,
            GSTIN = customer.GSTIN,
            IsActive = customer.IsActive
        };
    }
}