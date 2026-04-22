using LeaveManagementSystem.Business.Interfaces.Services;
using LeaveManagementSystem.Business.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LeaveManagementSystem.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddBusiness(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILeaveService, LeaveService>();
        services.AddScoped<IDashboardService, DashboardService>();

        return services;
    }
}
