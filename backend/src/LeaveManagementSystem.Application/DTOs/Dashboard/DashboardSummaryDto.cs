namespace LeaveManagementSystem.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public string Role { get; set; } = string.Empty;
    public int TotalLeaveRequests { get; set; }
    public int PendingLeaves { get; set; }
    public int ApprovedLeaves { get; set; }
    public int RejectedLeaves { get; set; }
    public int AvailableBalanceDays { get; set; }
    public int TeamPendingApprovals { get; set; }
    public int EmployeesCount { get; set; }
}
