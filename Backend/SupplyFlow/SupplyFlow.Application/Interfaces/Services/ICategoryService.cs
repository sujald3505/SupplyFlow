using SupplyFlow.Application.DTOs.Category;

namespace SupplyFlow.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllAsync();

    Task<CategoryDto> GetByIdAsync(int id);

    Task<CategoryDto> CreateAsync(
        CreateCategoryDto request);

    Task<CategoryDto> UpdateAsync(
        int id,
        UpdateCategoryDto request);
}