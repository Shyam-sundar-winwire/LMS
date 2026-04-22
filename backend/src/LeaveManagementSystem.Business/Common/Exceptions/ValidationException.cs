namespace LeaveManagementSystem.Business.Common.Exceptions;

public class ValidationException(string message) : AppException(message, 400);
