using LeaveManagementSystem.Application.DTOs.Dashboard;
using LeaveManagementSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary(CancellationToken cancellationToken)
    {
        return Ok(await dashboardService.GetSummaryAsync(cancellationToken));
    }
}
