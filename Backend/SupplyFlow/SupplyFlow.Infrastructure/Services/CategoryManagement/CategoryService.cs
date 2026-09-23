using Microsoft.EntityFrameworkCore;
using SupplyFlow.Application.DTOs.Category;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.CategoryManagement;

public class CategoryService : ICategoryService
{
    private readonly SupplyFlowDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CategoryService(
        SupplyFlowDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }


    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var companyId = GetCompanyId();

        return await _context.Categories
            .Where(x => x.CompanyId == companyId)
            .OrderBy(x => x.Name)
            .Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }


    public async Task<CategoryDto> GetByIdAsync(
        int id)
    {
        var companyId = GetCompanyId();

        var category = await _context.Categories
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId);

        if (category == null)
        {
            throw new KeyNotFoundException(
                "Category not found.");
        }

        return MapToDto(category);
    }


    public async Task<CategoryDto> CreateAsync(
        CreateCategoryDto request)
    {
        var companyId = GetCompanyId();

        var categoryName = request.Name.Trim();

        var exists = await _context.Categories
            .AnyAsync(x =>
                x.CompanyId == companyId &&
                x.Name == categoryName);

        if (exists)
        {
            throw new InvalidOperationException(
                "Category already exists.");
        }

        var category =
            new SupplyFlow.Domain.Entities.Category
            {
                CompanyId = companyId,
                Name = categoryName,
                Description =
                    request.Description?.Trim(),
                IsActive = true
            };

        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        return MapToDto(category);
    }


    public async Task<CategoryDto> UpdateAsync(
        int id,
        UpdateCategoryDto request)
    {
        var companyId = GetCompanyId();

        var category = await _context.Categories
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId);

        if (category == null)
        {
            throw new KeyNotFoundException(
                "Category not found.");
        }

        var categoryName = request.Name.Trim();

        var exists = await _context.Categories
            .AnyAsync(x =>
                x.Id != id &&
                x.CompanyId == companyId &&
                x.Name == categoryName);

        if (exists)
        {
            throw new InvalidOperationException(
                "Another category with this name already exists.");
        }

        category.Name = categoryName;
        category.Description =
            request.Description?.Trim();

        category.IsActive =
            request.IsActive;

        await _context.SaveChangesAsync();

        return MapToDto(category);
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


    private static CategoryDto MapToDto(
        SupplyFlow.Domain.Entities.Category category)
    {
        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsActive = category.IsActive
        };
    }
}