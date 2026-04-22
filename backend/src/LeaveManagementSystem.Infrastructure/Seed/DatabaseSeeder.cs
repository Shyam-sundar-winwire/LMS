using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Infrastructure.Persistence;
using LeaveManagementSystem.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Infrastructure.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var passwordHasher = new PasswordHasherService();
        var hasEmployees = await dbContext.Employees.AnyAsync(cancellationToken);

        if (!hasEmployees)
        {
            var admin = new Employee
            {
                Email = "admin@leaveapp.com",
                FullName = "Shyam sundar",
                RoleId = 4,
                Password = passwordHasher.Hash("Admin@123")
            };

            var manager = new Employee
            {
                Email = "manager@leaveapp.com",
                FullName = "Chaithanya",
                RoleId = 2,
                Password = passwordHasher.Hash("Manager@123")
            };

            var hr = new Employee
            {
                Email = "hr@leaveapp.com",
                FullName = "Nadia",
                RoleId = 3,
                Password = passwordHasher.Hash("HR@123")
            };

            var employee = new Employee
            {
                Email = "employee@leaveapp.com",
                FullName = "Ravi",
                RoleId = 1,
                Password = passwordHasher.Hash("Employee@123")
            };

            dbContext.Employees.AddRange(admin, manager, hr, employee);
            await dbContext.SaveChangesAsync(cancellationToken);

            employee.ManagerId = manager.Id;
            await dbContext.SaveChangesAsync(cancellationToken);

            var currentYear = DateTime.UtcNow.Year;
            var leaveTypes = await dbContext.LeaveTypes.AsNoTracking().ToListAsync(cancellationToken);
            dbContext.LeaveBalances.AddRange(
                leaveTypes.SelectMany(leaveType => new[]
                {
                    new LeaveBalance { EmployeeId = manager.Id, LeaveTypeId = leaveType.Id, Year = currentYear, RemainingDays = leaveType.DefaultDays },
                    new LeaveBalance { EmployeeId = hr.Id, LeaveTypeId = leaveType.Id, Year = currentYear, RemainingDays = leaveType.DefaultDays },
                    new LeaveBalance { EmployeeId = employee.Id, LeaveTypeId = leaveType.Id, Year = currentYear, RemainingDays = leaveType.DefaultDays }
                }));

            dbContext.LeaveRequests.Add(new LeaveRequest
            {
                EmployeeId = employee.Id,
                LeaveTypeId = 1,
                StartDate = new DateOnly(currentYear, 5, 14),
                EndDate = new DateOnly(currentYear, 5, 16),
                Status = LeaveStatuses.Pending,
                Reason = "Family function travel",
                CreatedAtUtc = DateTime.UtcNow.AddDays(-2)
            });

            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var expectedNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["admin@leaveapp.com"] = "Shyam sundar",
            ["manager@leaveapp.com"] = "Chaithanya",
            ["hr@leaveapp.com"] = "Nadia",
            ["employee@leaveapp.com"] = "Ravi"
        };

        var existingEmployees = await dbContext.Employees
            .Where(employee => expectedNames.Keys.Contains(employee.Email))
            .ToListAsync(cancellationToken);

        foreach (var existingEmployee in existingEmployees)
        {
            var expectedName = expectedNames[existingEmployee.Email];
            if (!string.Equals(existingEmployee.FullName, expectedName, StringComparison.Ordinal))
            {
                existingEmployee.FullName = expectedName;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
