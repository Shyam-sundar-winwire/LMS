using LeaveManagementSystem.Application.Interfaces.Repositories;
using LeaveManagementSystem.Infrastructure.Persistence;

namespace LeaveManagementSystem.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
