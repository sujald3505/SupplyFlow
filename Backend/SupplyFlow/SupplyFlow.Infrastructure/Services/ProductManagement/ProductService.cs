//using Microsoft.EntityFrameworkCore;
//using SupplyFlow.Application.DTOs.Product;
//using SupplyFlow.Application.Interfaces.Services;
//using SupplyFlow.Infrastructure.Persistence;

//namespace SupplyFlow.Infrastructure.Services.ProductManagement;

//public class ProductService : IProductService
//{
//    private readonly SupplyFlowDbContext _context;
//    private readonly ICurrentUserService _currentUser;

//    public ProductService(
//        SupplyFlowDbContext context,
//        ICurrentUserService currentUser)
//    {
//        _context = context;
//        _currentUser = currentUser;
//    }


//    public async Task<List<ProductDto>> GetAllAsync()
//    {
//        var companyId = GetCompanyId();

//        return await _context.Products
//            .Where(x => x.CompanyId == companyId)
//            .Include(x => x.Category)
//            .Include(x => x.Unit)
//            .OrderBy(x => x.Name)
//            .Select(x => new ProductDto
//            {
//                Id = x.Id,

//                CategoryId = x.CategoryId,
//                CategoryName = x.Category.Name,

//                UnitId = x.UnitId,
//                UnitName = x.Unit.Name,
//                UnitSymbol = x.Unit.Symbol,

//                Name = x.Name,
//                SKU = x.SKU,
//                Description = x.Description,

//                CostPrice = x.CostPrice,
//                SellingPrice = x.SellingPrice,
//                MinimumStockLevel = x.MinimumStockLevel,

//                IsActive = x.IsActive
//            })
//            .ToListAsync();
//    }


//    public async Task<ProductDto> GetByIdAsync(
//        int id)
//    {
//        var companyId = GetCompanyId();

//        var product = await _context.Products
//            .Include(x => x.Category)
//            .Include(x => x.Unit)
//            .FirstOrDefaultAsync(x =>
//                x.Id == id &&
//                x.CompanyId == companyId);

//        if (product == null)
//        {
//            throw new KeyNotFoundException(
//                "Product not found.");
//        }

//        return MapToDto(product);
//    }


//    public async Task<ProductDto> CreateAsync(
//        CreateProductDto request)
//    {
//        var companyId = GetCompanyId();

//        await ValidateCategoryAndUnitAsync(
//            request.CategoryId,
//            request.UnitId,
//            companyId);

//        var sku = request.SKU.Trim();

//        var skuExists = await _context.Products
//            .AnyAsync(x =>
//                x.CompanyId == companyId &&
//                x.SKU == sku);

//        if (skuExists)
//        {
//            throw new InvalidOperationException(
//                "Product SKU already exists.");
//        }

//        var product =
//            new SupplyFlow.Domain.Entities.Product
//            {
//                CompanyId = companyId,
//                CategoryId = request.CategoryId,
//                UnitId = request.UnitId,

//                Name = request.Name.Trim(),
//                SKU = sku,
//                Description =
//                    request.Description?.Trim(),

//                CostPrice = request.CostPrice,
//                SellingPrice = request.SellingPrice,
//                MinimumStockLevel =
//                    request.MinimumStockLevel,

//                IsActive = true
//            };

//        _context.Products.Add(product);

//        await _context.SaveChangesAsync();

//        // Load navigation properties for response
//        await _context.Entry(product)
//            .Reference(x => x.Category)
//            .LoadAsync();

//        await _context.Entry(product)
//            .Reference(x => x.Unit)
//            .LoadAsync();

//        return MapToDto(product);
//    }


//    public async Task<ProductDto> UpdateAsync(
//        int id,
//        UpdateProductDto request)
//    {
//        var companyId = GetCompanyId();

//        var product = await _context.Products
//            .FirstOrDefaultAsync(x =>
//                x.Id == id &&
//                x.CompanyId == companyId);

//        if (product == null)
//        {
//            throw new KeyNotFoundException(
//                "Product not found.");
//        }

//        await ValidateCategoryAndUnitAsync(
//            request.CategoryId,
//            request.UnitId,
//            companyId);

//        var sku = request.SKU.Trim();

//        var skuExists = await _context.Products
//            .AnyAsync(x =>
//                x.Id != id &&
//                x.CompanyId == companyId &&
//                x.SKU == sku);

//        if (skuExists)
//        {
//            throw new InvalidOperationException(
//                "Another product with this SKU already exists.");
//        }

//        product.CategoryId = request.CategoryId;
//        product.UnitId = request.UnitId;

//        product.Name = request.Name.Trim();
//        product.SKU = sku;
//        product.Description =
//            request.Description?.Trim();

//        product.CostPrice = request.CostPrice;
//        product.SellingPrice =
//            request.SellingPrice;

//        product.MinimumStockLevel =
//            request.MinimumStockLevel;

//        product.IsActive = request.IsActive;

//        await _context.SaveChangesAsync();

//        await _context.Entry(product)
//            .Reference(x => x.Category)
//            .LoadAsync();

//        await _context.Entry(product)
//            .Reference(x => x.Unit)
//            .LoadAsync();

//        return MapToDto(product);
//    }


//    private async Task ValidateCategoryAndUnitAsync(
//        int categoryId,
//        int unitId,
//        int companyId)
//    {
//        var categoryExists =
//            await _context.Categories
//                .AnyAsync(x =>
//                    x.Id == categoryId &&
//                    x.CompanyId == companyId &&
//                    x.IsActive);

//        if (!categoryExists)
//        {
//            throw new KeyNotFoundException(
//                "Category not found or inactive.");
//        }

//        var unitExists =
//            await _context.Units
//                .AnyAsync(x =>
//                    x.Id == unitId &&
//                    x.CompanyId == companyId &&
//                    x.IsActive);

//        if (!unitExists)
//        {
//            throw new KeyNotFoundException(
//                "Unit not found or inactive.");
//        }
//    }


//    private int GetCompanyId()
//    {
//        if (!_currentUser.CompanyId.HasValue)
//        {
//            throw new UnauthorizedAccessException(
//                "Company information not found.");
//        }

//        return _currentUser.CompanyId.Value;
//    }


//    private static ProductDto MapToDto(
//        SupplyFlow.Domain.Entities.Product product)
//    {
//        return new ProductDto
//        {
//            Id = product.Id,

//            CategoryId = product.CategoryId,
//            CategoryName = product.Category.Name,

//            UnitId = product.UnitId,
//            UnitName = product.Unit.Name,
//            UnitSymbol = product.Unit.Symbol,

//            Name = product.Name,
//            SKU = product.SKU,
//            Description = product.Description,

//            CostPrice = product.CostPrice,
//            SellingPrice = product.SellingPrice,
//            MinimumStockLevel =
//                product.MinimumStockLevel,

//            IsActive = product.IsActive
//        };
//    }
//}

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using SupplyFlow.Application.DTOs.Product;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.ProductManagement;

public class ProductService : IProductService
{
    private readonly SupplyFlowDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IWebHostEnvironment _environment;

    public ProductService(
        SupplyFlowDbContext context,
        ICurrentUserService currentUser,
        IWebHostEnvironment environment)
    {
        _context = context;
        _currentUser = currentUser;
        _environment = environment;
    }


    public async Task<List<ProductDto>> GetAllAsync()
    {
        var companyId = GetCompanyId();

        return await _context.Products
            .Where(x => x.CompanyId == companyId)
            .Include(x => x.Category)
            .Include(x => x.Unit)
            .OrderBy(x => x.Name)
            .Select(x => new ProductDto
            {
                Id = x.Id,

                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,

                UnitId = x.UnitId,
                UnitName = x.Unit.Name,
                UnitSymbol = x.Unit.Symbol,

                Name = x.Name,
                SKU = x.SKU,
                Description = x.Description,
                ImageUrl = x.ImageUrl,

                CostPrice = x.CostPrice,
                SellingPrice = x.SellingPrice,
                MinimumStockLevel = x.MinimumStockLevel,

                IsActive = x.IsActive
            })
            .ToListAsync();
    }


    public async Task<ProductDto> GetByIdAsync(
        int id)
    {
        var companyId = GetCompanyId();

        var product = await _context.Products
            .Include(x => x.Category)
            .Include(x => x.Unit)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId);

        if (product == null)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }

        return MapToDto(product);
    }


    public async Task<ProductDto> CreateAsync(
        CreateProductDto request)
    {
        var companyId = GetCompanyId();

        await ValidateCategoryAndUnitAsync(
            request.CategoryId,
            request.UnitId,
            companyId);

        var sku = request.SKU.Trim();

        var skuExists = await _context.Products
            .AnyAsync(x =>
                x.CompanyId == companyId &&
                x.SKU == sku);

        if (skuExists)
        {
            throw new InvalidOperationException(
                "Product SKU already exists.");
        }

        var product =
            new SupplyFlow.Domain.Entities.Product
            {
                CompanyId = companyId,
                CategoryId = request.CategoryId,
                UnitId = request.UnitId,

                Name = request.Name.Trim(),
                SKU = sku,
                Description =
                    request.Description?.Trim(),

                CostPrice = request.CostPrice,
                SellingPrice = request.SellingPrice,
                MinimumStockLevel =
                    request.MinimumStockLevel,

                IsActive = true
            };

        _context.Products.Add(product);

        await _context.SaveChangesAsync();

        // Load navigation properties for response
        await _context.Entry(product)
            .Reference(x => x.Category)
            .LoadAsync();

        await _context.Entry(product)
            .Reference(x => x.Unit)
            .LoadAsync();

        return MapToDto(product);
    }


    public async Task<ProductDto> UpdateAsync(
        int id,
        UpdateProductDto request)
    {
        var companyId = GetCompanyId();

        var product = await _context.Products
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId);

        if (product == null)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }

        await ValidateCategoryAndUnitAsync(
            request.CategoryId,
            request.UnitId,
            companyId);

        var sku = request.SKU.Trim();

        var skuExists = await _context.Products
            .AnyAsync(x =>
                x.Id != id &&
                x.CompanyId == companyId &&
                x.SKU == sku);

        if (skuExists)
        {
            throw new InvalidOperationException(
                "Another product with this SKU already exists.");
        }

        product.CategoryId = request.CategoryId;
        product.UnitId = request.UnitId;

        product.Name = request.Name.Trim();
        product.SKU = sku;
        product.Description =
            request.Description?.Trim();

        product.CostPrice = request.CostPrice;
        product.SellingPrice =
            request.SellingPrice;

        product.MinimumStockLevel =
            request.MinimumStockLevel;

        product.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        await _context.Entry(product)
            .Reference(x => x.Category)
            .LoadAsync();

        await _context.Entry(product)
            .Reference(x => x.Unit)
            .LoadAsync();

        return MapToDto(product);
    }


    public async Task<ProductDto> UploadImageAsync(
        int id,
        Stream imageStream,
        string fileName,
        string contentType)
    {
        var companyId = GetCompanyId();

        var product = await _context.Products
            .Include(x => x.Category)
            .Include(x => x.Unit)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId);

        if (product == null)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }

        var allowedContentTypes = new[]
        {
            "image/jpeg",
            "image/png",
            "image/webp"
        };

        if (!allowedContentTypes.Contains(contentType.ToLowerInvariant()))
        {
            throw new InvalidOperationException(
                "Only JPG, PNG and WEBP images are allowed.");
        }

        const long maxFileSize = 5 * 1024 * 1024;
        if (imageStream.Length > maxFileSize)
        {
            throw new InvalidOperationException(
                "Product image size must be 5 MB or less.");
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };

        if (!allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException(
                "Only JPG, PNG and WEBP images are allowed.");
        }

        var webRoot = _environment.WebRootPath;
        if (string.IsNullOrWhiteSpace(webRoot))
        {
            webRoot = Path.Combine(_environment.ContentRootPath, "wwwroot");
        }

        var uploadFolder = Path.Combine(
            webRoot,
            "uploads",
            "products");

        Directory.CreateDirectory(uploadFolder);

        if (!string.IsNullOrWhiteSpace(product.ImageUrl))
        {
            var oldRelativePath = product.ImageUrl
                .TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar);

            var oldFullPath = Path.Combine(webRoot, oldRelativePath);

            if (File.Exists(oldFullPath))
            {
                File.Delete(oldFullPath);
            }
        }

        var storedFileName = $"{product.Id}_{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(uploadFolder, storedFileName);

        await using (var fileStream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None))
        {
            await imageStream.CopyToAsync(fileStream);
        }

        product.ImageUrl = $"/uploads/products/{storedFileName}";

        await _context.SaveChangesAsync();

        return MapToDto(product);
    }


    private async Task ValidateCategoryAndUnitAsync(
        int categoryId,
        int unitId,
        int companyId)
    {
        var categoryExists =
            await _context.Categories
                .AnyAsync(x =>
                    x.Id == categoryId &&
                    x.CompanyId == companyId &&
                    x.IsActive);

        if (!categoryExists)
        {
            throw new KeyNotFoundException(
                "Category not found or inactive.");
        }

        var unitExists =
            await _context.Units
                .AnyAsync(x =>
                    x.Id == unitId &&
                    x.CompanyId == companyId &&
                    x.IsActive);

        if (!unitExists)
        {
            throw new KeyNotFoundException(
                "Unit not found or inactive.");
        }
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


    private static ProductDto MapToDto(
        SupplyFlow.Domain.Entities.Product product)
    {
        return new ProductDto
        {
            Id = product.Id,

            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,

            UnitId = product.UnitId,
            UnitName = product.Unit.Name,
            UnitSymbol = product.Unit.Symbol,

            Name = product.Name,
            SKU = product.SKU,
            Description = product.Description,
            ImageUrl = product.ImageUrl,

            CostPrice = product.CostPrice,
            SellingPrice = product.SellingPrice,
            MinimumStockLevel =
                product.MinimumStockLevel,

            IsActive = product.IsActive
        };
    }
}