using Microsoft.EntityFrameworkCore;
using SupplyFlow.Application.DTOs.PurchaseRequest;
using SupplyFlow.Application.Interfaces.Services;
using SupplyFlow.Domain.Enums;
using SupplyFlow.Infrastructure.Persistence;

namespace SupplyFlow.Infrastructure.Services.PurchaseRequestManagement;

public class PurchaseRequestService : IPurchaseRequestService
{
    private readonly SupplyFlowDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public PurchaseRequestService(
        SupplyFlowDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }


    public async Task<List<PurchaseRequestDto>> GetAllAsync()
    {
        var companyId = GetCompanyId();

        var requests = await _context.PurchaseRequests
            .Where(x => x.CompanyId == companyId)
            .Include(x => x.RequestedByUser)
            .Include(x => x.ApprovedByUser)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                    .ThenInclude(x => x.Unit)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return requests
            .Select(MapToDto)
            .ToList();
    }


    public async Task<PurchaseRequestDto> GetByIdAsync(
        int id)
    {
        var companyId = GetCompanyId();

        var request = await _context.PurchaseRequests
            .Include(x => x.RequestedByUser)
            .Include(x => x.ApprovedByUser)
            .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                    .ThenInclude(x => x.Unit)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                x.CompanyId == companyId);

        if (request == null)
        {
            throw new KeyNotFoundException(
                "Purchase request not found.");
        }

        return MapToDto(request);
    }


    public async Task<PurchaseRequestDto> CreateAsync(
        CreatePurchaseRequestDto request)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();

        if (request.Items == null ||
            request.Items.Count == 0)
        {
            throw new InvalidOperationException(
                "At least one product is required.");
        }

        // Check duplicate products
        var duplicateProduct =
            request.Items
                .GroupBy(x => x.ProductId)
                .Any(x => x.Count() > 1);

        if (duplicateProduct)
        {
            throw new InvalidOperationException(
                "Duplicate products are not allowed in a purchase request.");
        }

        // Validate quantities
        if (request.Items.Any(
            x => x.RequestedQuantity <= 0))
        {
            throw new InvalidOperationException(
                "Requested quantity must be greater than zero.");
        }

        var productIds =
            request.Items
                .Select(x => x.ProductId)
                .ToList();

        // Validate all products belong to company and are active
        var validProductsCount =
            await _context.Products
                .CountAsync(x =>
                    productIds.Contains(x.Id) &&
                    x.CompanyId == companyId &&
                    x.IsActive);

        if (validProductsCount !=
            productIds.Count)
        {
            throw new InvalidOperationException(
                "One or more products are invalid or inactive.");
        }

        var purchaseRequest =
            new SupplyFlow.Domain.Entities.PurchaseRequest
            {
                CompanyId = companyId,

                RequestNumber =
                    await GenerateRequestNumberAsync(
                        companyId),

                RequestedByUserId = userId,

                Status =
                    PurchaseRequestStatus.Draft,

                Remarks =
                    request.Remarks?.Trim()
            };

        foreach (var item in request.Items)
        {
            purchaseRequest.Items.Add(
                new SupplyFlow.Domain.Entities.PurchaseRequestItem
                {
                    ProductId = item.ProductId,
                    RequestedQuantity =
                        item.RequestedQuantity,
                    Remarks =
                        item.Remarks?.Trim()
                });
        }

        _context.PurchaseRequests
            .Add(purchaseRequest);

        await _context.SaveChangesAsync();

        return await GetByIdAsync(
            purchaseRequest.Id);
    }


    public async Task<PurchaseRequestDto> SubmitAsync(
        int id)
    {
        var companyId = GetCompanyId();

        var purchaseRequest =
            await _context.PurchaseRequests
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == companyId);

        if (purchaseRequest == null)
        {
            throw new KeyNotFoundException(
                "Purchase request not found.");
        }

        if (purchaseRequest.Status !=
            PurchaseRequestStatus.Draft)
        {
            throw new InvalidOperationException(
                "Only draft purchase requests can be submitted.");
        }

        purchaseRequest.Status =
            PurchaseRequestStatus.Submitted;

        purchaseRequest.SubmittedAt =
            DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }


    public async Task<PurchaseRequestDto> ApproveAsync(
        int id)
    {
        var companyId = GetCompanyId();
        var userId = GetUserId();

        var purchaseRequest =
            await _context.PurchaseRequests
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == companyId);

        if (purchaseRequest == null)
        {
            throw new KeyNotFoundException(
                "Purchase request not found.");
        }

        if (purchaseRequest.Status !=
            PurchaseRequestStatus.Submitted)
        {
            throw new InvalidOperationException(
                "Only submitted purchase requests can be approved.");
        }

        purchaseRequest.Status =
            PurchaseRequestStatus.Approved;

        purchaseRequest.ApprovedAt =
            DateTime.UtcNow;

        purchaseRequest.ApprovedByUserId =
            userId;

        purchaseRequest.RejectionReason =
            null;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }


    public async Task<PurchaseRequestDto> RejectAsync(
        int id,
        RejectPurchaseRequestDto request)
    {
        var companyId = GetCompanyId();

        if (string.IsNullOrWhiteSpace(
            request.RejectionReason))
        {
            throw new InvalidOperationException(
                "Rejection reason is required.");
        }

        var purchaseRequest =
            await _context.PurchaseRequests
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.CompanyId == companyId);

        if (purchaseRequest == null)
        {
            throw new KeyNotFoundException(
                "Purchase request not found.");
        }

        if (purchaseRequest.Status !=
            PurchaseRequestStatus.Submitted)
        {
            throw new InvalidOperationException(
                "Only submitted purchase requests can be rejected.");
        }

        purchaseRequest.Status =
            PurchaseRequestStatus.Rejected;

        purchaseRequest.RejectionReason =
            request.RejectionReason.Trim();

        purchaseRequest.ApprovedAt = null;
        purchaseRequest.ApprovedByUserId = null;

        await _context.SaveChangesAsync();

        return await GetByIdAsync(id);
    }


    private async Task<string>
        GenerateRequestNumberAsync(
            int companyId)
    {
        var count =
            await _context.PurchaseRequests
                .CountAsync(x =>
                    x.CompanyId == companyId);

        return $"PR-{DateTime.UtcNow:yyyyMMdd}-{count + 1:D4}";
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


    private int GetUserId()
    {
        if (!_currentUser.UserId.HasValue)
        {
            throw new UnauthorizedAccessException(
                "User information not found.");
        }

        return _currentUser.UserId.Value;
    }


    private static PurchaseRequestDto MapToDto(
        SupplyFlow.Domain.Entities.PurchaseRequest request)
    {
        return new PurchaseRequestDto
        {
            Id = request.Id,

            RequestNumber =
                request.RequestNumber,

            RequestedByUserId =
                request.RequestedByUserId,

            RequestedByUserName =
                request.RequestedByUser.FullName,

            Status =
                request.Status.ToString(),

            Remarks =
                request.Remarks,

            SubmittedAt =
                request.SubmittedAt,

            ApprovedAt =
                request.ApprovedAt,

            ApprovedByUserId =
                request.ApprovedByUserId,

            ApprovedByUserName =
                request.ApprovedByUser?.FullName,

            RejectionReason =
                request.RejectionReason,

            CreatedAt =
                request.CreatedAt,

            Items = request.Items
                .Select(item =>
                    new PurchaseRequestItemDto
                    {
                        Id = item.Id,

                        ProductId =
                            item.ProductId,

                        ProductName =
                            item.Product.Name,

                        SKU =
                            item.Product.SKU,

                        RequestedQuantity =
                            item.RequestedQuantity,

                        UnitName =
                            item.Product.Unit.Name,

                        UnitSymbol =
                            item.Product.Unit.Symbol,

                        Remarks =
                            item.Remarks
                    })
                .ToList()
        };
    }
}