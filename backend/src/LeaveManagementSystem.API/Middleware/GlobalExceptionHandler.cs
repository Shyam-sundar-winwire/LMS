using System.Text.Json;
using LeaveManagementSystem.Business.Common.Exceptions;

namespace LeaveManagementSystem.API.Middleware;

public class GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            // Single line to handle all exceptions
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new
        {
            message = GetErrorMessage(exception),
            statusCode = GetStatusCode(exception),
            traceId = context.TraceIdentifier,
            timestamp = DateTime.UtcNow
        };

        response.StatusCode = errorResponse.statusCode;

        logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }

    private static int GetStatusCode(Exception exception) => exception switch
    {
        ValidationException => 400,
        ForbiddenException => 403,
        NotFoundException => 404,
        BadHttpRequestException badRequest => badRequest.StatusCode,
        OperationCanceledException => 499,
        _ => 500
    };

    private static string GetErrorMessage(Exception exception) => exception switch
    {
        ValidationException validation => validation.Message,
        ForbiddenException forbidden => forbidden.Message,
        NotFoundException notFound => notFound.Message,
        BadHttpRequestException badRequest => badRequest.Message,
        OperationCanceledException => "Request was cancelled by the client.",
        _ => "An unexpected error occurred. Please try again later."
    };
}
