using LeaveManagementSystem.Business.Common.Exceptions;
using LeaveManagementSystem.Infrastructure.Security;
using Xunit;

namespace LeaveManagementSystem.Business.Tests;

public class PasswordHasherServiceTests
{
    [Fact]
    public void Hash_WithBlankPassword_ThrowsValidationException()
    {
        var sut = new PasswordHasherService();

        var exception = Assert.Throws<ValidationException>(() => sut.Hash(" "));

        Assert.Equal("Password is required.", exception.Message);
    }

    [Fact]
    public void HashAndVerify_WithValidPassword_ReturnsTrue()
    {
        var sut = new PasswordHasherService();

        var hash = sut.Hash("Password@123");

        Assert.True(sut.Verify(hash, "Password@123"));
    }

    [Fact]
    public void Verify_WithBlankValues_ReturnsFalse()
    {
        var sut = new PasswordHasherService();

        Assert.False(sut.Verify("", "Password@123"));
        Assert.False(sut.Verify("hash", ""));
    }
}
