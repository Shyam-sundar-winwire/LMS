using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Business.DTOs.LeaveRequests;

public class UpdateLeaveRequestStatusDto
{
    public bool Approve { get; set; }

    [StringLength(500)]
    public string? Comment { get; set; }
}
