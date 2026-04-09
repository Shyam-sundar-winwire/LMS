using LeaveManagementSystem.Application.DTOs.Dashboard;

namespace LeaveManagementSystem.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);
}
