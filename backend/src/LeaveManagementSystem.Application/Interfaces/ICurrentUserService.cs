namespace LeaveManagementSystem.Application.Interfaces;

public interface ICurrentUserService
{
    int GetUserId();
    string GetRole();
}
