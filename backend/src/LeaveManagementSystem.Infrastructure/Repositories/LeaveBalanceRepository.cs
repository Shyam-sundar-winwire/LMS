using LeaveManagementSystem.Business.Interfaces.Repositories;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Infrastructure.Repositories;

public class LeaveBalanceRepository(AppDbContext dbContext) : ILeaveBalanceRepository
{
    public Task<LeaveBalance?> GetAsync(int employeeId, int leaveTypeId, int year, CancellationToken cancellationToken = default)
    {
        return dbContext.LeaveBalances
            .Include(x => x.LeaveType)
            .FirstOrDefaultAsync(
                x => x.EmployeeId == employeeId && x.LeaveTypeId == leaveTypeId && x.Year == year,
                cancellationToken);
    }

    public async Task<IReadOnlyList<LeaveBalance>> GetByEmployeeAsync(int employeeId, int year, CancellationToken cancellationToken = default)
    {
        return await dbContext.LeaveBalances
            .Include(x => x.LeaveType)
            .Where(x => x.EmployeeId == employeeId && x.Year == year)
            .OrderBy(x => x.LeaveType!.Name)
            .ToListAsync(cancellationToken);
    }

    public Task AddAsync(LeaveBalance leaveBalance, CancellationToken cancellationToken = default)
    {
        return dbContext.LeaveBalances.AddAsync(leaveBalance, cancellationToken).AsTask();
    }
}
