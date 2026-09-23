using SupplyFlow.Application.DTOs.PurchaseRequest;

namespace SupplyFlow.Application.Interfaces.Services;

public interface IPurchaseRequestService
{
    Task<List<PurchaseRequestDto>> GetAllAsync();

    Task<PurchaseRequestDto> GetByIdAsync(int id);

    Task<PurchaseRequestDto> CreateAsync(
        CreatePurchaseRequestDto request);

    Task<PurchaseRequestDto> SubmitAsync(int id);

    Task<PurchaseRequestDto> ApproveAsync(int id);

    Task<PurchaseRequestDto> RejectAsync(
        int id,
        RejectPurchaseRequestDto request);
}