using SupplyFlow.Application.DTOs.Dashboard;

namespace SupplyFlow.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardAsync();
}