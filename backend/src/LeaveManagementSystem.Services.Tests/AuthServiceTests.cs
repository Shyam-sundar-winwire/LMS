using LeaveManagementSystem.Business.Common.Exceptions;
using LeaveManagementSystem.Business.DTOs.Auth;
using LeaveManagementSystem.Business.Interfaces.Repositories;
using LeaveManagementSystem.Business.Interfaces.Security;
using LeaveManagementSystem.Business.Services;
using LeaveManagementSystem.Domain.Entities;
using Moq;
using Xunit;

namespace LeaveManagementSystem.Business.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_WithNullRequest_ThrowsValidationException()
    {
        var sut = new AuthService(
            Mock.Of<IEmployeeRepository>(),
            Mock.Of<IPasswordHasher>(),
            Mock.Of<IJwtTokenService>());

        var exception = await Assert.ThrowsAsync<ValidationException>(() => sut.LoginAsync(null!));

        Assert.Equal("Login request is required.", exception.Message);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ThrowsValidationException()
    {
        var employeeRepository = new Mock<IEmployeeRepository>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher>(MockBehavior.Strict);
        var jwtTokenService = new Mock<IJwtTokenService>(MockBehavior.Strict);

        employeeRepository
            .Setup(repository => repository.GetByEmailAsync("user@demo.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Employee { Id = 1, Email = "user@demo.com", Password = "hashed" });

        passwordHasher
            .Setup(hasher => hasher.Verify("hashed", "wrong-password"))
            .Returns(false);

        var sut = new AuthService(employeeRepository.Object, passwordHasher.Object, jwtTokenService.Object);

        var exception = await Assert.ThrowsAsync<ValidationException>(() => sut.LoginAsync(new LoginRequestDto
        {
            Email = "user@demo.com",
            Password = "wrong-password"
        }));

        Assert.Equal("Invalid email or password.", exception.Message);
        employeeRepository.VerifyAll();
        passwordHasher.VerifyAll();
        jwtTokenService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_NormalizesEmail_AndReturnsToken()
    {
        var employeeRepository = new Mock<IEmployeeRepository>(MockBehavior.Strict);
        var passwordHasher = new Mock<IPasswordHasher>(MockBehavior.Strict);
        var jwtTokenService = new Mock<IJwtTokenService>(MockBehavior.Strict);

        var employee = new Employee
        {
            Id = 42,
            Email = "employee@leaveapp.com",
            Password = "hashed-password",
            FullName = "Ravi",
            Role = new Role { Name = "Employee" }
        };

        var expectedResponse = new LoginResponseDto
        {
            Token = "jwt-token",
            ExpiresAtUtc = new DateTime(2030, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            User = new UserProfileDto
            {
                Id = 42,
                Email = "employee@leaveapp.com",
                FullName = "Ravi",
                Role = "Employee"
            }
        };

        employeeRepository
            .Setup(repository => repository.GetByEmailAsync("employee@leaveapp.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(employee);

        passwordHasher
            .Setup(hasher => hasher.Verify("hashed-password", "Password@123"))
            .Returns(true);

        jwtTokenService
            .Setup(service => service.GenerateToken(employee))
            .Returns(expectedResponse);

        var sut = new AuthService(employeeRepository.Object, passwordHasher.Object, jwtTokenService.Object);

        var result = await sut.LoginAsync(new LoginRequestDto
        {
            Email = "  EMPLOYEE@leaveapp.com  ",
            Password = "Password@123"
        });

        Assert.Same(expectedResponse, result);
        employeeRepository.VerifyAll();
        passwordHasher.VerifyAll();
        jwtTokenService.VerifyAll();
    }
}
