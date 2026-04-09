namespace LeaveManagementSystem.Application.Common.Exceptions;

public class NotFoundException(string message) : AppException(message, 404);
