using System.Security.Claims;
using System.Text.Json;
using LeaveManagementSystem.API.Controllers;
using LeaveManagementSystem.API.Middleware;
using LeaveManagementSystem.Business.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LeaveManagementSystem.Business.Tests;

public class HealthAndMiddlewareAdditionalTests
{
    [Fact]
    public async Task HealthController_Get_ReturnsOkForHealthyReport()
    {
        using var services = new ServiceCollection()
            .AddLogging()
            .AddHealthChecks()
            .Services
            .BuildServiceProvider();

        var controller = new HealthController(
            services.GetRequiredService<HealthCheckService>(),
            Mock.Of<ILogger<HealthController>>());

        var result = await controller.Get();

        var ok = Assert.IsType<OkObjectResult>(result);
        using var document = JsonSerializer.SerializeToDocument(ok.Value);
        Assert.Equal("ok", document.RootElement.GetProperty("status").GetString());
        Assert.Equal("LeaveManagementSystem.API", document.RootElement.GetProperty("service").GetString());
    }

    [Fact]
    public async Task HealthController_Get_ReturnsDegradedWhenAnyCheckIsDegraded()
    {
        using var services = new ServiceCollection()
            .AddLogging()
            .AddHealthChecks()
            .AddCheck("slow_dependency", () => HealthCheckResult.Degraded("slow"))
            .Services
            .BuildServiceProvider();

        var controller = new HealthController(
            services.GetRequiredService<HealthCheckService>(),
            Mock.Of<ILogger<HealthController>>());

        var result = await controller.Get();

        var ok = Assert.IsType<OkObjectResult>(result);
        using var document = JsonSerializer.SerializeToDocument(ok.Value);
        Assert.Equal("degraded", document.RootElement.GetProperty("status").GetString());
        Assert.Equal("slow_dependency", document.RootElement.GetProperty("checks")[0].GetProperty("name").GetString());
    }

    [Fact]
    public void HealthController_ReadyAndLive_ReturnOkPayloads()
    {
        var controller = new HealthController();

        var ready = Assert.IsType<OkObjectResult>(controller.Ready());
        var live = Assert.IsType<OkObjectResult>(controller.Live());

        using var readyDocument = JsonSerializer.SerializeToDocument(ready.Value);
        using var liveDocument = JsonSerializer.SerializeToDocument(live.Value);
        Assert.Equal("ready", readyDocument.RootElement.GetProperty("status").GetString());
        Assert.Equal("alive", liveDocument.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task RequestLoggingMiddleware_InvokesNextAndLogsCompletion()
    {
        var context = new DefaultHttpContext();
        context.Request.Method = HttpMethods.Post;
        context.Request.Path = "/api/test";
        context.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.Name, "user@demo.com")], "Test"));

        var logger = new Mock<ILogger<RequestLoggingMiddleware>>();
        var middleware = new RequestLoggingMiddleware(
            ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                return Task.CompletedTask;
            },
            logger.Object);

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
        logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((_, _) => true),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Theory]
    [InlineData(typeof(ValidationException), StatusCodes.Status400BadRequest, "bad")]
    [InlineData(typeof(ForbiddenException), StatusCodes.Status403Forbidden, "no")]
    [InlineData(typeof(NotFoundException), StatusCodes.Status404NotFound, "missing")]
    [InlineData(typeof(BadHttpRequestException), StatusCodes.Status418ImATeapot, "tea")]
    [InlineData(typeof(OperationCanceledException), 499, "Request was cancelled by the client.")]
    [InlineData(typeof(InvalidOperationException), StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.")]
    public async Task GlobalExceptionHandler_MapsExceptionsToJsonResponses(Type exceptionType, int expectedStatusCode, string expectedMessage)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var middleware = new GlobalExceptionHandler(
            _ => throw CreateException(exceptionType),
            Mock.Of<ILogger<GlobalExceptionHandler>>());

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        Assert.Equal(expectedStatusCode, context.Response.StatusCode);
        Assert.Equal(expectedMessage, document.RootElement.GetProperty("message").GetString());
        Assert.Equal(expectedStatusCode, document.RootElement.GetProperty("statusCode").GetInt32());
    }

    [Fact]
    public async Task ExceptionHandlingMiddleware_WhenRequestAborted_ReturnsClientClosedStatus()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        var context = new DefaultHttpContext();
        context.RequestAborted = cancellationTokenSource.Token;
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new OperationCanceledException(cancellationTokenSource.Token),
            Mock.Of<ILogger<ExceptionHandlingMiddleware>>());

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status499ClientClosedRequest, context.Response.StatusCode);
    }

    private static Exception CreateException(Type exceptionType)
    {
        if (exceptionType == typeof(ValidationException))
        {
            return new ValidationException("bad");
        }

        if (exceptionType == typeof(ForbiddenException))
        {
            return new ForbiddenException("no");
        }

        if (exceptionType == typeof(NotFoundException))
        {
            return new NotFoundException("missing");
        }

        if (exceptionType == typeof(BadHttpRequestException))
        {
            return new BadHttpRequestException("tea", StatusCodes.Status418ImATeapot);
        }

        if (exceptionType == typeof(OperationCanceledException))
        {
            return new OperationCanceledException();
        }

        return new InvalidOperationException("boom");
    }
}
