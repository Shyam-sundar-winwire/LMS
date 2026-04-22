using LeaveManagementSystem.Business.Common.Exceptions;
using LeaveManagementSystem.Business.DTOs.Auth;
using LeaveManagementSystem.Business.Interfaces.Repositories;
using LeaveManagementSystem.Business.Interfaces.Security;
using LeaveManagementSystem.Business.Interfaces.Services;

namespace LeaveManagementSystem.Business.Services;

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
