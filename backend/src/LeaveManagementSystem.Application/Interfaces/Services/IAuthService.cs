using LeaveManagementSystem.Application.DTOs.Auth;

namespace LeaveManagementSystem.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default);
}
