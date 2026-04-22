namespace LeaveManagementSystem.Business.DTOs.LeaveRequests;

public class ApplyLeaveRequestDto
{
    public int LeaveTypeId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
}
