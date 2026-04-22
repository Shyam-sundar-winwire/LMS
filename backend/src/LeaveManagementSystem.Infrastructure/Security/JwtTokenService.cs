using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LeaveManagementSystem.Business.DTOs.Auth;
using LeaveManagementSystem.Business.Interfaces.Security;
using LeaveManagementSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace LeaveManagementSystem.Infrastructure.Security;

public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public LoginResponseDto GenerateToken(Employee employee)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var secretKey = jwtSection["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is missing.");
        var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("JWT Issuer is missing.");
        var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("JWT Audience is missing.");
        var expiryMinutes = int.TryParse(jwtSection["ExpiryMinutes"], out var parsed) ? parsed : 120;

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(expiryMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, employee.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, employee.Email),
            new(ClaimTypes.NameIdentifier, employee.Id.ToString()),
            new(ClaimTypes.Name, employee.FullName),
            new(ClaimTypes.Role, employee.Role?.Name ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new LoginResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc = expiresAtUtc,
            User = BuildUserProfile(employee)
        };
    }

    public UserProfileDto BuildUserProfile(Employee employee)
    {
        return new UserProfileDto
        {
            Id = employee.Id,
            Email = employee.Email,
            FullName = employee.FullName,
            Role = employee.Role?.Name ?? string.Empty,
            ManagerId = employee.ManagerId
        };
    }
}
