using System.Diagnostics;

namespace LeaveManagementSystem.API.Middleware;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString("N")[..8];
        
        // Log request start
        logger.LogInformation("[{RequestId}] {Method} {Path} started - User: {UserId}", 
            requestId, context.Request.Method, context.Request.Path, 
            context.User?.Identity?.Name ?? "Anonymous");

        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            var logLevel = statusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
            
            logger.Log(logLevel, "[{RequestId}] {Method} {Path} completed - Status: {StatusCode}, Duration: {Duration}ms", 
                requestId, context.Request.Method, context.Request.Path, statusCode, stopwatch.ElapsedMilliseconds);
        }
    }
}
