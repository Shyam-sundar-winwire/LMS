using LeaveManagementSystem.Business.DTOs.Auth;
using LeaveManagementSystem.Domain.Entities;

namespace LeaveManagementSystem.Business.Interfaces.Security;

public interface IJwtTokenService
{
    LoginResponseDto GenerateToken(Employee employee);
    UserProfileDto BuildUserProfile(Employee employee);
}
