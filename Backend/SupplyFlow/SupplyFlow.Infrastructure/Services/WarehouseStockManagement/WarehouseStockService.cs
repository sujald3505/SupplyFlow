using Microsoft.EntityFrameworkCore;
using SupplyFlow.Application.DTOs.WarehouseStock;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.WarehouseStockManagement;

public class WarehouseStockService : IWarehouseStockService
{
    private readonly SupplyFlowDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public WarehouseStockService(
        SupplyFlowDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }


    public async Task<List<WarehouseStockDto>> GetAllAsync()
    {
        var companyId = GetCompanyId();

        return await _context.WarehouseStocks
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.Warehouse.Name)
            .ThenBy(x => x.Product.Name)
            .Select(x => new WarehouseStockDto
            {
                Id = x.Id,

                WarehouseId = x.WarehouseId,
                WarehouseName = x.Warehouse.Name,

                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                SKU = x.Product.SKU,

                UnitName = x.Product.Unit.Name,
                UnitSymbol = x.Product.Unit.Symbol,

                Quantity = x.Quantity
            })
            .ToListAsync();
    }


    public async Task<List<WarehouseStockDto>>
        GetByWarehouseIdAsync(int warehouseId)
    {
        var companyId = GetCompanyId();

        var warehouseExists = await _context.Warehouses
            .AnyAsync(x =>
                x.Id == warehouseId &&
                x.CompanyId == companyId);

        if (!warehouseExists)
        {
            throw new KeyNotFoundException(
                "Warehouse not found.");
        }

        return await _context.WarehouseStocks
            .Where(x =>
                x.CompanyId == companyId &&
                x.WarehouseId == warehouseId)
            .OrderBy(x => x.Product.Name)
            .Select(x => new WarehouseStockDto
            {
                Id = x.Id,

                WarehouseId = x.WarehouseId,
                WarehouseName = x.Warehouse.Name,

                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                SKU = x.Product.SKU,

                UnitName = x.Product.Unit.Name,
                UnitSymbol = x.Product.Unit.Symbol,

                Quantity = x.Quantity
            })
            .ToListAsync();
    }


    public async Task<List<WarehouseStockDto>>
        GetByProductIdAsync(int productId)
    {
        var companyId = GetCompanyId();

        var productExists = await _context.Products
            .AnyAsync(x =>
                x.Id == productId &&
                x.CompanyId == companyId);

        if (!productExists)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }

        return await _context.WarehouseStocks
            .Where(x =>
                x.CompanyId == companyId &&
                x.ProductId == productId)
            .OrderBy(x => x.Warehouse.Name)
            .Select(x => new WarehouseStockDto
            {
                Id = x.Id,

                WarehouseId = x.WarehouseId,
                WarehouseName = x.Warehouse.Name,

                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                SKU = x.Product.SKU,

                UnitName = x.Product.Unit.Name,
                UnitSymbol = x.Product.Unit.Symbol,

                Quantity = x.Quantity
            })
            .ToListAsync();
    }


    public async Task<List<LowStockDto>>
        GetLowStockAsync()
    {
        var companyId = GetCompanyId();

        return await _context.WarehouseStocks
            .Where(x =>
                x.CompanyId == companyId &&
                x.Quantity < x.Product.MinimumStockLevel)
            .OrderBy(x => x.Product.Name)
            .Select(x => new LowStockDto
            {
                WarehouseId = x.WarehouseId,
                WarehouseName = x.Warehouse.Name,

                ProductId = x.ProductId,
                ProductName = x.Product.Name,
                SKU = x.Product.SKU,

                CurrentQuantity = x.Quantity,

                MinimumStockLevel =
                    x.Product.MinimumStockLevel,

                ShortageQuantity =
                    x.Product.MinimumStockLevel -
                    x.Quantity,

                UnitSymbol =
                    x.Product.Unit.Symbol
            })
            .ToListAsync();
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
}