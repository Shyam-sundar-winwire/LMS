using LeaveManagementSystem.Application.Common.Exceptions;
using LeaveManagementSystem.Application.DTOs.Auth;
using LeaveManagementSystem.Application.Interfaces.Repositories;
using LeaveManagementSystem.Application.Interfaces.Security;
using LeaveManagementSystem.Application.Interfaces.Services;

namespace LeaveManagementSystem.Application.Services;

public class AuthService(
    IEmployeeRepository employeeRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : IAuthService
{
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var employee = await employeeRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant(), cancellationToken);
        if (employee is null || !passwordHasher.Verify(employee.Password, request.Password))
        {
            throw new ValidationException("Invalid email or password.");
        }

        return jwtTokenService.GenerateToken(employee);
    }
}
