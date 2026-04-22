using LeaveManagementSystem.Domain.Enums;

namespace LeaveManagementSystem.Domain.Common;

public static class LeaveStatuses
{
    public const string Pending = nameof(LeaveRequestStatusType.Pending);
    public const string Approved = nameof(LeaveRequestStatusType.Approved);
    public const string Rejected = nameof(LeaveRequestStatusType.Rejected);
}
