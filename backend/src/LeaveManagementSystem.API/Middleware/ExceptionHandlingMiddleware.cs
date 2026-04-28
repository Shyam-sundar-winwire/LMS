using System.Text.Json;
using LeaveManagementSystem.Business.Common.Exceptions;

namespace LeaveManagementSystem.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppException exception)
        {
            await WriteErrorResponseAsync(context, exception.StatusCode, exception.Message);
        }
        catch (BadHttpRequestException exception)
        {
            logger.LogWarning(exception, "Bad request while processing {Method} {Path}.", context.Request.Method, context.Request.Path);
            await WriteErrorResponseAsync(context, exception.StatusCode, exception.Message);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            logger.LogInformation("Request was cancelled by the client for {Method} {Path}.", context.Request.Method, context.Request.Path);

            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = StatusCodes.Status499ClientClosedRequest;
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unhandled exception while processing request.");
            await WriteErrorResponseAsync(context, StatusCodes.Status500InternalServerError, "An unexpected server error occurred.");
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, int statusCode, string message)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            message,
            statusCode,
            traceId = context.TraceIdentifier
        }));
    }
}
