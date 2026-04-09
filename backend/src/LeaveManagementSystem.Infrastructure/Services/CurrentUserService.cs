using System.Security.Claims;
using LeaveManagementSystem.Application.Common.Exceptions;
using LeaveManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LeaveManagementSystem.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public int GetUserId()
    {
        var rawValue = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(rawValue, out var userId))
        {
            throw new ForbiddenException("Unable to identify the current user.");
        }

        return userId;
    }

    public string GetRole()
    {
        var role = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
        if (string.IsNullOrWhiteSpace(role))
        {
            throw new ForbiddenException("Unable to identify the current user's role.");
        }

        return role;
    }
}
