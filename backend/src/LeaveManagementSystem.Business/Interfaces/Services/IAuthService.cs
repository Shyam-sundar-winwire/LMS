using LeaveManagementSystem.Business.DTOs.Auth;

namespace LeaveManagementSystem.Business.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
