namespace LeaveManagementSystem.Domain.Entities;

public class LeaveType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DefaultDays { get; set; }
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
}
