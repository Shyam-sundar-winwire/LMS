using LeaveManagementSystem.Business.Common.Exceptions;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace LeaveManagementSystem.Business.Tests;

public class JwtTokenServiceTests
{
    [Fact]
    public void GenerateToken_WithValidEmployee_ReturnsTokenAndProfile()
    {
        var sut = new JwtTokenService(BuildConfiguration());

        var result = sut.GenerateToken(new Employee
        {
            Id = 1,
            Email = "employee@demo.com",
            FullName = "Ravi Kumar",
            ManagerId = 4,
            Role = new Role { Name = "Employee" }
        });

        Assert.False(string.IsNullOrWhiteSpace(result.Token));
        Assert.Equal("employee@demo.com", result.User.Email);
        Assert.Equal("Employee", result.User.Role);
        Assert.Equal(4, result.User.ManagerId);
    }

    [Fact]
    public void GenerateToken_WithoutRole_ThrowsValidationException()
    {
        var sut = new JwtTokenService(BuildConfiguration());

        var exception = Assert.Throws<ValidationException>(() => sut.GenerateToken(new Employee
        {
            Id = 1,
            Email = "employee@demo.com",
            FullName = "Ravi Kumar"
        }));

        Assert.Equal("Employee role is required to generate a token.", exception.Message);
    }

    [Fact]
    public void BuildUserProfile_WithNullEmployee_ThrowsValidationException()
    {
        var sut = new JwtTokenService(BuildConfiguration());

        var exception = Assert.Throws<ValidationException>(() => sut.BuildUserProfile(null!));

        Assert.Equal("Employee is required to build the user profile.", exception.Message);
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = "A_Very_Strong_Secret_Key_For_Leave_Management_System_2026",
                ["Jwt:Issuer"] = "LeaveManagementSystem",
                ["Jwt:Audience"] = "LeaveManagementSystem.Client",
                ["Jwt:ExpiryMinutes"] = "120"
            })
            .Build();
    }
}
