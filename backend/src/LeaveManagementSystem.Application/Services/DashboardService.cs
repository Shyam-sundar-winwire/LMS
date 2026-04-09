using LeaveManagementSystem.Application.DTOs.Dashboard;
using LeaveManagementSystem.Application.Interfaces;
using LeaveManagementSystem.Application.Interfaces.Repositories;
using LeaveManagementSystem.Application.Interfaces.Services;
using LeaveManagementSystem.Domain.Common;

namespace LeaveManagementSystem.Application.Services;

public class DashboardService(
    ICurrentUserService currentUserService,
    IEmployeeRepository employeeRepository,
    ILeaveRequestRepository leaveRequestRepository,
    ILeaveBalanceRepository leaveBalanceRepository) : IDashboardService
{
    public async Task<DashboardSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.GetUserId();
        var role = currentUserService.GetRole();
        var currentYear = DateTime.UtcNow.Year;
        var balances = await leaveBalanceRepository.GetByEmployeeAsync(userId, currentYear, cancellationToken);

        var summary = new DashboardSummaryDto
        {
            Role = role,
            AvailableBalanceDays = balances.Sum(x => x.RemainingDays)
        };

        if (role == RoleNames.Employee)
        {
            summary.TotalLeaveRequests = (await leaveRequestRepository.GetByEmployeeAsync(userId, cancellationToken)).Count;
            summary.PendingLeaves = await leaveRequestRepository.CountByStatusForEmployeeAsync(userId, LeaveStatuses.Pending, cancellationToken);
            summary.ApprovedLeaves = await leaveRequestRepository.CountByStatusForEmployeeAsync(userId, LeaveStatuses.Approved, cancellationToken);
            summary.RejectedLeaves = await leaveRequestRepository.CountByStatusForEmployeeAsync(userId, LeaveStatuses.Rejected, cancellationToken);
            return summary;
        }

        if (role == RoleNames.Manager)
        {
            summary.TeamPendingApprovals = (await leaveRequestRepository.GetPendingForManagerAsync(userId, cancellationToken)).Count;
            summary.PendingLeaves = summary.TeamPendingApprovals;
            summary.TotalLeaveRequests = (await leaveRequestRepository.GetByEmployeeAsync(userId, cancellationToken)).Count;
            summary.ApprovedLeaves = await leaveRequestRepository.CountByStatusForEmployeeAsync(userId, LeaveStatuses.Approved, cancellationToken);
            summary.RejectedLeaves = await leaveRequestRepository.CountByStatusForEmployeeAsync(userId, LeaveStatuses.Rejected, cancellationToken);
            return summary;
        }

        summary.TotalLeaveRequests = (await leaveRequestRepository.GetAllAsync(cancellationToken)).Count;
        summary.PendingLeaves = await leaveRequestRepository.CountByStatusAsync(LeaveStatuses.Pending, cancellationToken);
        summary.ApprovedLeaves = await leaveRequestRepository.CountByStatusAsync(LeaveStatuses.Approved, cancellationToken);
        summary.RejectedLeaves = await leaveRequestRepository.CountByStatusAsync(LeaveStatuses.Rejected, cancellationToken);
        summary.EmployeesCount = await employeeRepository.CountAsync(cancellationToken);

        return summary;
    }
}
