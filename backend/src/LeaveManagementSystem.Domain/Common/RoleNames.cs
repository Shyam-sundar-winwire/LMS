using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Domain.Common;

public static class RoleNames
{
    public const string Employee = nameof(RoleType.Employee);
    public const string Manager = nameof(RoleType.Manager);
    public const string Hr = "HR";
    public const string Admin = nameof(RoleType.Admin);

    public static readonly string[] All = [Employee, Manager, Hr, Admin];
}
