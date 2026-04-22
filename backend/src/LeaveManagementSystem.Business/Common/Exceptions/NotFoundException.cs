namespace LeaveManagementSystem.Business.Common.Exceptions;

public class NotFoundException(string message) : AppException(message, 404);
