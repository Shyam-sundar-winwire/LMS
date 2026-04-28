using LeaveManagementSystem.Business.Common.Exceptions;
using LeaveManagementSystem.Business.DTOs.Dashboard;
using LeaveManagementSystem.Business.Interfaces;
using LeaveManagementSystem.Business.Interfaces.Repositories;
using LeaveManagementSystem.Business.Interfaces.Services;
using LeaveManagementSystem.Domain.Common;

namespace LeaveManagementSystem.Business.Services;

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
            var teamRequests = (await leaveRequestRepository.GetAllAsync(cancellationToken))
                .Where(request => request.Employee?.ManagerId == userId)
                .ToList();

            summary.TeamPendingApprovals = teamRequests.Count(request => request.Status == LeaveStatuses.Pending);
            summary.PendingLeaves = summary.TeamPendingApprovals;
            summary.TotalLeaveRequests = (await leaveRequestRepository.GetByEmployeeAsync(userId, cancellationToken)).Count;
            summary.ApprovedLeaves = teamRequests.Count(request => request.Status == LeaveStatuses.Approved);
            summary.RejectedLeaves = teamRequests.Count(request => request.Status == LeaveStatuses.Rejected);
            return summary;
        }

        if (role is not (RoleNames.Hr or RoleNames.Admin))
        {
            throw new ForbiddenException("You are not authorized to view organization-wide dashboard data.");
        }

        summary.TotalLeaveRequests = (await leaveRequestRepository.GetAllAsync(cancellationToken)).Count;
        summary.PendingLeaves = await leaveRequestRepository.CountByStatusAsync(LeaveStatuses.Pending, cancellationToken);
        summary.ApprovedLeaves = await leaveRequestRepository.CountByStatusAsync(LeaveStatuses.Approved, cancellationToken);
        summary.RejectedLeaves = await leaveRequestRepository.CountByStatusAsync(LeaveStatuses.Rejected, cancellationToken);
        summary.EmployeesCount = await employeeRepository.CountAsync(cancellationToken);

        return summary;
    }
}
