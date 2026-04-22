namespace LeaveManagementSystem.Business.DTOs.LeaveRequests;

public class LeaveBalanceDto
{
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public int Year { get; set; }
    public int RemainingDays { get; set; }
}
