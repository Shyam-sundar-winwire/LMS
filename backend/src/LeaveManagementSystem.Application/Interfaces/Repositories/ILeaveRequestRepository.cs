using LeaveManagementSystem.Domain.Entities;

namespace LeaveManagementSystem.Application.Interfaces.Repositories;

public interface ILeaveRequestRepository
{
    Task AddAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default);
    Task<LeaveRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveRequest>> GetByEmployeeAsync(int employeeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveRequest>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveRequest>> GetPendingForManagerAsync(int managerId, CancellationToken cancellationToken = default);
    Task<int> CountByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<int> CountByStatusForEmployeeAsync(int employeeId, string status, CancellationToken cancellationToken = default);
}
