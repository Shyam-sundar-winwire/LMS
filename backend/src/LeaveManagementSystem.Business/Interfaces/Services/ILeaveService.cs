using LeaveManagementSystem.Business.DTOs.LeaveRequests;

namespace LeaveManagementSystem.Business.Interfaces.Services;

public interface ILeaveService
{
    Task<LeaveRequestDto> ApplyLeaveAsync(ApplyLeaveRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveRequestDto>> GetMyLeavesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveRequestDto>> GetPendingApprovalsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveRequestDto>> GetAllLeavesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveBalanceDto>> GetMyLeaveBalancesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveTypeDto>> GetLeaveTypesAsync(CancellationToken cancellationToken = default);
    Task<LeaveRequestDto> ReviewLeaveAsync(int leaveRequestId, UpdateLeaveRequestStatusDto request, CancellationToken cancellationToken = default);
}
