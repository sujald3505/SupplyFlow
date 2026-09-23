using SupplyFlow.Application.DTOs.PurchaseOrder;

namespace SupplyFlow.Application.Interfaces.Services;

public interface IPurchaseOrderService
{
    Task<List<PurchaseOrderDto>> GetAllAsync();

    Task<PurchaseOrderDto> GetByIdAsync(int id);

    Task<PurchaseOrderDto> CreateAsync(
        CreatePurchaseOrderDto request);

    Task<PurchaseOrderDto> SendAsync(int id);

    Task<PurchaseOrderDto> ConfirmAsync(int id);

    Task<PurchaseOrderDto> CancelAsync(int id);
}