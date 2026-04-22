using LeaveManagementSystem.Domain.Entities;

namespace LeaveManagementSystem.Business.Interfaces.Repositories;

public interface ILeaveBalanceRepository
{
    Task<LeaveBalance?> GetAsync(int employeeId, int leaveTypeId, int year, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveBalance>> GetByEmployeeAsync(int employeeId, int year, CancellationToken cancellationToken = default);
    Task AddAsync(LeaveBalance leaveBalance, CancellationToken cancellationToken = default);
}
