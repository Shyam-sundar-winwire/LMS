namespace LeaveManagementSystem.Business.Interfaces;

public interface ICurrentUserService
{
    int GetUserId();
    string GetRole();
}
