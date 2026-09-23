//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using SupplyFlow.Application.DTOs.Product;
//using SupplyFlow.Application.Interfaces.Services;

//namespace SupplyFlow.API.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//[Authorize]
//public class ProductsController : ControllerBase
//{
//    private readonly IProductService _productService;

//    public ProductsController(
//        IProductService productService)
//    {
//        _productService = productService;
//    }


//    [HttpGet]
//    public async Task<IActionResult> GetAll()
//    {
//        var products =
//            await _productService.GetAllAsync();

//        return Ok(products);
//    }


//    [HttpGet("{id:int}")]
//    public async Task<IActionResult> GetById(
//        int id)
//    {
//        var product =
//            await _productService.GetByIdAsync(id);

//        return Ok(product);
//    }


//    [HttpPost]
//    [Authorize(Roles =
//        "Admin,WarehouseManager,PurchaseManager")]
//    public async Task<IActionResult> Create(
//        CreateProductDto request)
//    {
//        var product =
//            await _productService.CreateAsync(request);

//        return Ok(product);
//    }


//    [HttpPut("{id:int}")]
//    [Authorize(Roles =
//        "Admin,WarehouseManager,PurchaseManager")]
//    public async Task<IActionResult> Update(
//        int id,
//        UpdateProductDto request)
//    {
//        var product =
//            await _productService.UpdateAsync(
//                id,
//                request);

//        return Ok(product);
//    }
//}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupplyFlow.Application.DTOs.Product;
using SupplyFlow.Application.Interfaces.Services;

namespace SupplyFlow.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(
        IProductService productService)
    {
        _productService = productService;
    }


    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products =
            await _productService.GetAllAsync();

        return Ok(products);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id)
    {
        var product =
            await _productService.GetByIdAsync(id);

        return Ok(product);
    }


    [HttpPost]
    [Authorize(Roles =
        "Admin,WarehouseManager,PurchaseManager")]
    public async Task<IActionResult> Create(
        CreateProductDto request)
    {
        var product =
            await _productService.CreateAsync(request);

        return Ok(product);
    }


    [HttpPost("{id:int}/image")]
    [Authorize(Roles = "Admin,WarehouseManager,PurchaseManager")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadImage(
        int id,
        IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return BadRequest("Product image is required.");
        }

        await using var stream = image.OpenReadStream();

        var product = await _productService.UploadImageAsync(
            id,
            stream,
            image.FileName,
            image.ContentType);

        return Ok(product);
    }


    [HttpPut("{id:int}")]
    [Authorize(Roles =
        "Admin,WarehouseManager,PurchaseManager")]
    public async Task<IActionResult> Update(
        int id,
        UpdateProductDto request)
    {
        var product =
            await _productService.UpdateAsync(
                id,
                request);

        return Ok(product);
    }
}