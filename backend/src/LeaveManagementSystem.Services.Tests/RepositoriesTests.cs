using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Infrastructure.Persistence;
using LeaveManagementSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LeaveManagementSystem.Business.Tests;

public class RepositoriesTests
{
    [Fact]
    public async Task EmployeeRepository_GetByEmailAsync_ReturnsEmployeeWithRole()
    {
        await using var dbContext = CreateDbContext();
        var role = new Role { Id = 10, Name = "Employee" };
        dbContext.Roles.Add(role);
        dbContext.Employees.Add(new Employee { Id = 1, Email = "employee@demo.com", Password = "hash", FullName = "Demo Employee", RoleId = 10, Role = role });
        await dbContext.SaveChangesAsync();

        var repository = new EmployeeRepository(dbContext);

        var employee = await repository.GetByEmailAsync("employee@demo.com");

        Assert.NotNull(employee);
        Assert.Equal("Employee", employee!.Role!.Name);
    }

    [Fact]
    public async Task LeaveBalanceRepository_GetByEmployeeAsync_ReturnsOrderedBalances()
    {
        await using var dbContext = CreateDbContext();
        var sick = new LeaveType { Id = 1, Name = "Sick Leave", DefaultDays = 10 };
        var annual = new LeaveType { Id = 2, Name = "Annual Leave", DefaultDays = 18 };
        dbContext.LeaveTypes.AddRange(sick, annual);
        dbContext.LeaveBalances.AddRange(
            new LeaveBalance { EmployeeId = 1, LeaveTypeId = 1, Year = 2026, RemainingDays = 5, LeaveType = sick },
            new LeaveBalance { EmployeeId = 1, LeaveTypeId = 2, Year = 2026, RemainingDays = 9, LeaveType = annual });
        await dbContext.SaveChangesAsync();

        var repository = new LeaveBalanceRepository(dbContext);

        var balances = await repository.GetByEmployeeAsync(1, 2026);

        Assert.Equal(["Annual Leave", "Sick Leave"], balances.Select(x => x.LeaveType!.Name).ToArray());
    }

    [Fact]
    public async Task LeaveBalanceRepository_GetAsync_ReturnsMatchingBalance()
    {
        await using var dbContext = CreateDbContext();
        var role = new Role { Id = 1, Name = "Employee" };
        var employee = new Employee { Id = 1, Email = "employee@demo.com", Password = "hash", FullName = "Employee", RoleId = 1, Role = role };
        var leaveType = new LeaveType { Id = 2, Name = "Sick Leave", DefaultDays = 10 };
        dbContext.Roles.Add(role);
        dbContext.Employees.Add(employee);
        dbContext.LeaveTypes.Add(leaveType);
        dbContext.LeaveBalances.Add(new LeaveBalance { EmployeeId = 1, LeaveTypeId = 2, Year = 2026, RemainingDays = 7, Employee = employee, LeaveType = leaveType });
        await dbContext.SaveChangesAsync();

        var repository = new LeaveBalanceRepository(dbContext);

        var balance = await repository.GetAsync(1, 2, 2026);

        Assert.NotNull(balance);
        Assert.Equal(7, balance!.RemainingDays);
    }

    [Fact]
    public async Task LeaveRequestRepository_GetPendingForManagerAsync_FiltersManagerPendingItems()
    {
        await using var dbContext = CreateDbContext();
        var role = new Role { Id = 1, Name = "Employee" };
        var manager = new Employee { Id = 100, Email = "manager@demo.com", Password = "hash", FullName = "Manager", RoleId = 1, Role = role };
        var employee = new Employee { Id = 1, Email = "employee@demo.com", Password = "hash", FullName = "Employee", RoleId = 1, Role = role, ManagerId = 100, Manager = manager };
        var otherEmployee = new Employee { Id = 2, Email = "other@demo.com", Password = "hash", FullName = "Other", RoleId = 1, Role = role, ManagerId = 200 };
        var leaveType = new LeaveType { Id = 1, Name = "Annual Leave", DefaultDays = 18 };
        dbContext.Roles.Add(role);
        dbContext.Employees.AddRange(manager, employee, otherEmployee);
        dbContext.LeaveTypes.Add(leaveType);
        dbContext.LeaveRequests.AddRange(
            new LeaveRequest { Id = 1, EmployeeId = 1, Employee = employee, LeaveTypeId = 1, LeaveType = leaveType, Status = LeaveStatuses.Pending, Reason = "Trip", StartDate = new DateOnly(2026, 5, 1), EndDate = new DateOnly(2026, 5, 2), CreatedAtUtc = DateTime.UtcNow },
            new LeaveRequest { Id = 2, EmployeeId = 1, Employee = employee, LeaveTypeId = 1, LeaveType = leaveType, Status = LeaveStatuses.Approved, Reason = "Trip", StartDate = new DateOnly(2026, 5, 3), EndDate = new DateOnly(2026, 5, 4), CreatedAtUtc = DateTime.UtcNow },
            new LeaveRequest { Id = 3, EmployeeId = 2, Employee = otherEmployee, LeaveTypeId = 1, LeaveType = leaveType, Status = LeaveStatuses.Pending, Reason = "Trip", StartDate = new DateOnly(2026, 5, 5), EndDate = new DateOnly(2026, 5, 6), CreatedAtUtc = DateTime.UtcNow });
        await dbContext.SaveChangesAsync();

        var repository = new LeaveRequestRepository(dbContext);

        var pending = await repository.GetPendingForManagerAsync(100);

        Assert.Single(pending);
        Assert.Equal(1, pending[0].Id);
    }

    [Fact]
    public async Task LeaveRequestRepository_GetByEmployeeAsync_ReturnsEmployeeRequests()
    {
        await using var dbContext = CreateDbContext();
        var role = new Role { Id = 1, Name = "Employee" };
        var employee = new Employee { Id = 1, Email = "employee@demo.com", Password = "hash", FullName = "Employee", RoleId = 1, Role = role };
        var leaveType = new LeaveType { Id = 1, Name = "Annual Leave", DefaultDays = 18 };
        dbContext.Roles.Add(role);
        dbContext.Employees.Add(employee);
        dbContext.LeaveTypes.Add(leaveType);
        dbContext.LeaveRequests.Add(new LeaveRequest { Id = 1, EmployeeId = 1, Employee = employee, LeaveTypeId = 1, LeaveType = leaveType, Status = LeaveStatuses.Pending, Reason = "Trip", StartDate = new DateOnly(2026, 5, 1), EndDate = new DateOnly(2026, 5, 2), CreatedAtUtc = DateTime.UtcNow });
        await dbContext.SaveChangesAsync();

        var repository = new LeaveRequestRepository(dbContext);

        var requests = await repository.GetByEmployeeAsync(1);

        Assert.Single(requests);
    }

    [Fact]
    public async Task LeaveRequestRepository_GetAllAsync_ReturnsAllRequests()
    {
        await using var dbContext = CreateDbContext();
        var role = new Role { Id = 1, Name = "Employee" };
        var employee = new Employee { Id = 1, Email = "employee@demo.com", Password = "hash", FullName = "Employee", RoleId = 1, Role = role };
        var leaveType = new LeaveType { Id = 1, Name = "Annual Leave", DefaultDays = 18 };
        dbContext.Roles.Add(role);
        dbContext.Employees.Add(employee);
        dbContext.LeaveTypes.Add(leaveType);
        dbContext.LeaveRequests.AddRange(
            new LeaveRequest { Id = 1, EmployeeId = 1, Employee = employee, LeaveTypeId = 1, LeaveType = leaveType, Status = LeaveStatuses.Pending, Reason = "Trip", StartDate = new DateOnly(2026, 5, 1), EndDate = new DateOnly(2026, 5, 2), CreatedAtUtc = DateTime.UtcNow },
            new LeaveRequest { Id = 2, EmployeeId = 1, Employee = employee, LeaveTypeId = 1, LeaveType = leaveType, Status = LeaveStatuses.Approved, Reason = "Trip", StartDate = new DateOnly(2026, 5, 3), EndDate = new DateOnly(2026, 5, 4), CreatedAtUtc = DateTime.UtcNow });
        await dbContext.SaveChangesAsync();

        var repository = new LeaveRequestRepository(dbContext);

        var requests = await repository.GetAllAsync();

        Assert.Equal(2, requests.Count);
    }

    [Fact]
    public async Task LeaveTypeRepository_GetAllAsync_ReturnsOrderedTypes()
    {
        await using var dbContext = CreateDbContext();
        dbContext.LeaveTypes.AddRange(
            new LeaveType { Id = 1, Name = "Sick Leave", DefaultDays = 10 },
            new LeaveType { Id = 2, Name = "Annual Leave", DefaultDays = 18 });
        await dbContext.SaveChangesAsync();

        var repository = new LeaveTypeRepository(dbContext);

        var leaveTypes = await repository.GetAllAsync();

        Assert.Equal(["Annual Leave", "Sick Leave"], leaveTypes.Select(x => x.Name).ToArray());
    }

    [Fact]
    public async Task LeaveTypeRepository_GetByIdAsync_ReturnsMatchingType()
    {
        await using var dbContext = CreateDbContext();
        dbContext.LeaveTypes.Add(new LeaveType { Id = 3, Name = "Casual Leave", DefaultDays = 7 });
        await dbContext.SaveChangesAsync();

        var repository = new LeaveTypeRepository(dbContext);

        var leaveType = await repository.GetByIdAsync(3);

        Assert.NotNull(leaveType);
        Assert.Equal("Casual Leave", leaveType!.Name);
    }

    [Fact]
    public async Task UnitOfWork_SaveChangesAsync_PersistsEntities()
    {
        await using var dbContext = CreateDbContext();
        var unitOfWork = new UnitOfWork(dbContext);

        dbContext.LeaveTypes.Add(new LeaveType { Id = 1, Name = "Annual Leave", DefaultDays = 18 });
        await unitOfWork.SaveChangesAsync();

        Assert.Equal(1, await dbContext.LeaveTypes.CountAsync());
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
