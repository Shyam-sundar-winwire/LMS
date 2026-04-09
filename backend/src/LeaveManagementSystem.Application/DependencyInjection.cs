using LeaveManagementSystem.Application.Interfaces.Services;
using LeaveManagementSystem.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LeaveManagementSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILeaveService, LeaveService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
