namespace LeaveManagementSystem.Domain.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public int? ManagerId { get; set; }
    public string FullName { get; set; } = string.Empty;

    public Role? Role { get; set; }
    public Employee? Manager { get; set; }
    public ICollection<Employee> DirectReports { get; set; } = new List<Employee>();
    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
}
