namespace LeaveManagementSystem.Application.DTOs.LeaveRequests;

public class UpdateLeaveRequestStatusDto
{
    public bool Approve { get; set; }
    public string? Comment { get; set; }
}
