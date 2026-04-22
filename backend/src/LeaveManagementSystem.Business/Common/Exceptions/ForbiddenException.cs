namespace LeaveManagementSystem.Business.Common.Exceptions;

public class ForbiddenException(string message) : AppException(message, 403);
