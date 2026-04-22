using LeaveManagementSystem.Business.Interfaces.Repositories;
using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Infrastructure.Repositories;

public class LeaveRequestRepository(AppDbContext dbContext) : ILeaveRequestRepository
{
    public Task AddAsync(LeaveRequest leaveRequest, CancellationToken cancellationToken = default)
    {
        return dbContext.LeaveRequests.AddAsync(leaveRequest, cancellationToken).AsTask();
    }

    public Task<LeaveRequest?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return Queryable().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<LeaveRequest>> GetByEmployeeAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        return await Queryable()
            .Where(x => x.EmployeeId == employeeId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LeaveRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Queryable()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LeaveRequest>> GetPendingForManagerAsync(int managerId, CancellationToken cancellationToken = default)
    {
        return await Queryable()
            .Where(x => x.Status == LeaveStatuses.Pending && x.Employee!.ManagerId == managerId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return dbContext.LeaveRequests.CountAsync(x => x.Status == status, cancellationToken);
    }

    public Task<int> CountByStatusForEmployeeAsync(int employeeId, string status, CancellationToken cancellationToken = default)
    {
        return dbContext.LeaveRequests.CountAsync(x => x.EmployeeId == employeeId && x.Status == status, cancellationToken);
    }

    private IQueryable<LeaveRequest> Queryable()
    {
        return dbContext.LeaveRequests
            .Include(x => x.Employee!)
                .ThenInclude(x => x!.Role)
            .Include(x => x.LeaveType)
            .Include(x => x.ReviewedByEmployee);
    }
}
