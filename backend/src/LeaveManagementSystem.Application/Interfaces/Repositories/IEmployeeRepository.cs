using LeaveManagementSystem.Domain.Entities;

namespace LeaveManagementSystem.Application.Interfaces.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
}
