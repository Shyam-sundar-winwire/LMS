namespace LeaveManagementSystem.Application.DTOs.Auth;

public class UserProfileDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? ManagerId { get; set; }
}
