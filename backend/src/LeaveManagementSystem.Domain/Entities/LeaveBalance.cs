namespace LeaveManagementSystem.Domain.Entities;

public class LeaveBalance
{
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public int Year { get; set; }
    public int RemainingDays { get; set; }

    public Employee? Employee { get; set; }
    public LeaveType? LeaveType { get; set; }
}
