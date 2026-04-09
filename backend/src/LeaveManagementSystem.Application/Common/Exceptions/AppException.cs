namespace LeaveManagementSystem.Application.Common.Exceptions;

public class AppException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
