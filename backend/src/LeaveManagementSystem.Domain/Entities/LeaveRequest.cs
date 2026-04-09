namespace LeaveManagementSystem.Domain.Entities;

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public int? ReviewedByEmployeeId { get; set; }
    public string? ManagerComment { get; set; }

    public Employee? Employee { get; set; }
    public LeaveType? LeaveType { get; set; }
    public Employee? ReviewedByEmployee { get; set; }
}
