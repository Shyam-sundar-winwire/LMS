namespace LeaveManagementSystem.Application.Common.Exceptions;

public class ForbiddenException(string message) : AppException(message, 403);
