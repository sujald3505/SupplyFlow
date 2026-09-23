//using SupplyFlow.Application.DTOs.Product;

//namespace SupplyFlow.Application.Interfaces.Services;

//public interface IProductService
//{
//    Task<List<ProductDto>> GetAllAsync();

//    Task<ProductDto> GetByIdAsync(int id);

//    Task<ProductDto> CreateAsync(
//        CreateProductDto request);

//    Task<ProductDto> UpdateAsync(
//        int id,
//        UpdateProductDto request);
//}

using SupplyFlow.Application.DTOs.Product;

namespace SupplyFlow.Application.Interfaces.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync();

    Task<ProductDto> GetByIdAsync(int id);

    Task<ProductDto> CreateAsync(
        CreateProductDto request);

    Task<ProductDto> UpdateAsync(
        int id,
        UpdateProductDto request);

    Task<ProductDto> UploadImageAsync(
        int id,
        Stream imageStream,
        string fileName,
        string contentType);
}