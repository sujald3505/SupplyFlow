using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.Category;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(
        ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories =
            await _categoryService.GetAllAsync();

        return Ok(categories);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category =
            await _categoryService.GetByIdAsync(id);

        return Ok(category);
    }


    [HttpPost]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Create(
        CreateCategoryDto request)
    {
        var category =
            await _categoryService.CreateAsync(request);

        return Ok(category);
    }


    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,WarehouseManager")]
    public async Task<IActionResult> Update(
        int id,
        UpdateCategoryDto request)
    {
        var category =
            await _categoryService.UpdateAsync(
                id,
                request);

        return Ok(category);
    }
}