using System.Text.Json;
using LeaveManagementSystem.API.Middleware;
using LeaveManagementSystem.Business.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LeaveManagementSystem.Business.Tests;

public class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WithAppException_ReturnsExpectedStatusCodeAndMessage()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new ValidationException("Validation failed."),
            Mock.Of<ILogger<ExceptionHandlingMiddleware>>());

        await middleware.InvokeAsync(httpContext);

        httpContext.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(httpContext.Response.Body);

        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
        Assert.Equal("Validation failed.", document.RootElement.GetProperty("message").GetString());
    }

    [Fact]
    public async Task InvokeAsync_WithBadHttpRequestException_ReturnsClientError()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new BadHttpRequestException("Bad request.", StatusCodes.Status400BadRequest),
            Mock.Of<ILogger<ExceptionHandlingMiddleware>>());

        await middleware.InvokeAsync(httpContext);

        Assert.Equal(StatusCodes.Status400BadRequest, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task InvokeAsync_WithUnhandledException_ReturnsInternalServerError()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Response.Body = new MemoryStream();

        var middleware = new ExceptionHandlingMiddleware(
            _ => throw new InvalidOperationException("Boom"),
            Mock.Of<ILogger<ExceptionHandlingMiddleware>>());

        await middleware.InvokeAsync(httpContext);

        httpContext.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(httpContext.Response.Body);

        Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);
        Assert.Equal("An unexpected server error occurred.", document.RootElement.GetProperty("message").GetString());
    }
}
