using LeaveManagementSystem.Application.DTOs.Auth;
using LeaveManagementSystem.Domain.Entities;

namespace LeaveManagementSystem.Application.Interfaces.Security;

public interface IJwtTokenService
{
    LoginResponseDto GenerateToken(Employee employee);
    UserProfileDto BuildUserProfile(Employee employee);
}
