using LeaveManagementSystem.Application.Interfaces;
using LeaveManagementSystem.Application.Interfaces.Repositories;
using LeaveManagementSystem.Application.Interfaces.Security;
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
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

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
