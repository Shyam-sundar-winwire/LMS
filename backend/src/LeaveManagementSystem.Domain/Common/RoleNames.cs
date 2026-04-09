namespace LeaveManagementSystem.Domain.Common;

public static class RoleNames
{
    public const string Employee = "Employee";
    public const string Manager = "Manager";
    public const string Hr = "HR";
    public const string Admin = "Admin";

    public static readonly string[] All = [Employee, Manager, Hr, Admin];
}
