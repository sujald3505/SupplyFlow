namespace SupplyFlow.Application.Interfaces.Services;

public interface ICurrentUserService
{
    int? UserId { get; }

    int? CompanyId { get; }

    string? Email { get; }

    string? Role { get; }

    bool IsAuthenticated { get; }
}