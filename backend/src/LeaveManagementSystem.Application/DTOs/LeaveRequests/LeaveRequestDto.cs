namespace LeaveManagementSystem.Application.DTOs.LeaveRequests;

public class LeaveRequestDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeEmail { get; set; } = string.Empty;
    public int LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int DaysRequested { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public string? ManagerComment { get; set; }
}
