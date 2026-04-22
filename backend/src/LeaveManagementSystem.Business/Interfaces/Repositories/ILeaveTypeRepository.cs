using LeaveManagementSystem.Domain.Entities;

namespace LeaveManagementSystem.Business.Interfaces.Repositories;

public interface ILeaveTypeRepository
{
    Task<LeaveType?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LeaveType>> GetAllAsync(CancellationToken cancellationToken = default);
}
