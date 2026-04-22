using LeaveManagementSystem.Business.DTOs.Dashboard;

namespace LeaveManagementSystem.Business.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}
