using System.Security.Claims;
using LeaveManagementSystem.Business.Common.Exceptions;
using LeaveManagementSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace LeaveManagementSystem.Business.Tests;

public class CurrentUserServiceTests
{
    [Fact]
    public void GetUserId_WithValidClaim_ReturnsUserId()
    {
        var contextAccessor = BuildContextAccessor([
            new Claim(ClaimTypes.NameIdentifier, "12"),
            new Claim(ClaimTypes.Role, "Employee")
        ]);

        var sut = new CurrentUserService(contextAccessor.Object);

        Assert.Equal(12, sut.GetUserId());
    }

    [Fact]
    public void GetUserId_WithoutNumericClaim_ThrowsForbiddenException()
    {
        var contextAccessor = BuildContextAccessor([new Claim(ClaimTypes.NameIdentifier, "abc")]);

        var sut = new CurrentUserService(contextAccessor.Object);

        var exception = Assert.Throws<ForbiddenException>(() => sut.GetUserId());
        Assert.Equal("Unable to identify the current user.", exception.Message);
    }

    [Fact]
    public void GetRole_WithoutRoleClaim_ThrowsForbiddenException()
    {
        var contextAccessor = BuildContextAccessor([new Claim(ClaimTypes.NameIdentifier, "5")]);

        var sut = new CurrentUserService(contextAccessor.Object);

        var exception = Assert.Throws<ForbiddenException>(() => sut.GetRole());
        Assert.Equal("Unable to identify the current user's role.", exception.Message);
    }

    private static Mock<IHttpContextAccessor> BuildContextAccessor(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, "test");
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(identity)
        };

        var accessor = new Mock<IHttpContextAccessor>(MockBehavior.Strict);
        accessor.SetupGet(x => x.HttpContext).Returns(context);
        return accessor;
    }
}
