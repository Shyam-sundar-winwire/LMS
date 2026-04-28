using LeaveManagementSystem.Business.Interfaces;
using LeaveManagementSystem.Business.Interfaces.Repositories;
using LeaveManagementSystem.Business.Interfaces.Security;
using LeaveManagementSystem.Infrastructure.Persistence;
using LeaveManagementSystem.Infrastructure.Repositories;
using LeaveManagementSystem.Infrastructure.Security;
using LeaveManagementSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LeaveManagementSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Default database connection string is missing.");
        }

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<ILeaveBalanceRepository, LeaveBalanceRepository>();
        services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
        services.AddScoped<ILeaveTypeRepository, LeaveTypeRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher, PasswordHasherService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}
