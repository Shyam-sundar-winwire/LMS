using LeaveManagementSystem.Application.Interfaces.Repositories;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Infrastructure.Repositories;

public class LeaveTypeRepository(AppDbContext dbContext) : ILeaveTypeRepository
{
    public Task<LeaveType?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.LeaveTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<LeaveType>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.LeaveTypes
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }
}
