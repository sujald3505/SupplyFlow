using SupplyFlow.Application.DTOs.GoodsReceipt;

namespace SupplyFlow.Application.Interfaces.Services;

public interface IGoodsReceiptService
{
    Task<List<GoodsReceiptDto>> GetAllAsync();

    Task<GoodsReceiptDto> GetByIdAsync(int id);

    Task<GoodsReceiptDto> CreateAsync(
        CreateGoodsReceiptDto request);

    Task<GoodsReceiptDto> CompleteAsync(int id);

    Task<GoodsReceiptDto> CancelAsync(int id);
}