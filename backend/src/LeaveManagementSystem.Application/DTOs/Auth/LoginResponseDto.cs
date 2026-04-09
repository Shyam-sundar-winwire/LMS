namespace LeaveManagementSystem.Application.DTOs.Auth;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public UserProfileDto User { get; set; } = new();
}
