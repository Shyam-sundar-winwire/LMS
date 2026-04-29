using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Infrastructure.Persistence;
using LeaveManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LeaveManagementSystem.Business.Tests;

public class RepositoryAdditionalTests
{
    [Fact]
    public async Task EmployeeRepository_GetByIdAndCount_ReturnExpectedValues()
    {
        await using var dbContext = CreateDbContext();
        var role = new Role { Id = 1, Name = RoleNames.Employee };
        dbContext.Roles.Add(role);
        dbContext.Employees.AddRange(
            new Employee { Id = 1, Email = "one@demo.com", Password = "hash", FullName = "One", RoleId = 1, Role = role },
            new Employee { Id = 2, Email = "two@demo.com", Password = "hash", FullName = "Two", RoleId = 1, Role = role });
        await dbContext.SaveChangesAsync();

        var repository = new EmployeeRepository(dbContext);

        var employee = await repository.GetByIdAsync(2);

        Assert.Equal(2, await repository.CountAsync());
        Assert.NotNull(employee);
        Assert.Equal(RoleNames.Employee, employee!.Role!.Name);
    }

    [Fact]
    public async Task LeaveRequestRepository_AddGetByIdAndCountMethods_WorkTogether()
    {
        await using var dbContext = CreateDbContext();
        var role = new Role { Id = 1, Name = RoleNames.Employee };
        var employee = new Employee { Id = 1, Email = "employee@demo.com", Password = "hash", FullName = "Employee", RoleId = 1, Role = role };
        var leaveType = new LeaveType { Id = 1, Name = "Annual Leave", DefaultDays = 18 };
        dbContext.Roles.Add(role);
        dbContext.Employees.Add(employee);
        dbContext.LeaveTypes.Add(leaveType);
        await dbContext.SaveChangesAsync();

        var repository = new LeaveRequestRepository(dbContext);
        await repository.AddAsync(new LeaveRequest
        {
            Id = 10,
            EmployeeId = 1,
            Employee = employee,
            LeaveTypeId = 1,
            LeaveType = leaveType,
            Status = LeaveStatuses.Pending,
            Reason = "Trip",
            StartDate = new DateOnly(2026, 5, 1),
            EndDate = new DateOnly(2026, 5, 2),
            CreatedAtUtc = DateTime.UtcNow
        });
        await dbContext.SaveChangesAsync();

        var request = await repository.GetByIdAsync(10);

        Assert.NotNull(request);
        Assert.Equal("Annual Leave", request!.LeaveType!.Name);
        Assert.Equal(1, await repository.CountByStatusAsync(LeaveStatuses.Pending));
        Assert.Equal(1, await repository.CountByStatusForEmployeeAsync(1, LeaveStatuses.Pending));
        Assert.Equal(0, await repository.CountByStatusForEmployeeAsync(2, LeaveStatuses.Pending));
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
