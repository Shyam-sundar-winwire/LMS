using LeaveManagementSystem.Business.DTOs.LeaveRequests;
using LeaveManagementSystem.Business.Interfaces.Services;
using LeaveManagementSystem.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class LeaveRequestsController(ILeaveService leaveService) : ControllerBase
{
    [HttpGet("mine")]
    [Authorize(Roles = $"{RoleNames.Employee},{RoleNames.Manager},{RoleNames.Hr},{RoleNames.Admin}")]
    public async Task<ActionResult<IReadOnlyList<LeaveRequestDto>>> GetMine(CancellationToken cancellationToken)
    {
        return Ok(await leaveService.GetMyLeavesAsync(cancellationToken));
    }

    [HttpGet("all")]
    [Authorize(Roles = $"{RoleNames.Hr},{RoleNames.Admin}")]
    public async Task<ActionResult<IReadOnlyList<LeaveRequestDto>>> GetAll(CancellationToken cancellationToken)
    {
        return Ok(await leaveService.GetAllLeavesAsync(cancellationToken));
    }

    [HttpGet("approvals")]
    [Authorize(Roles = $"{RoleNames.Manager},{RoleNames.Hr},{RoleNames.Admin}")]
    public async Task<ActionResult<IReadOnlyList<LeaveRequestDto>>> GetApprovals(CancellationToken cancellationToken)
    {
        return Ok(await leaveService.GetPendingApprovalsAsync(cancellationToken));
    }

    [HttpGet("balances")]
    [Authorize(Roles = $"{RoleNames.Employee},{RoleNames.Manager},{RoleNames.Hr},{RoleNames.Admin}")]
    public async Task<ActionResult<IReadOnlyList<LeaveBalanceDto>>> GetBalances(CancellationToken cancellationToken)
    {
        return Ok(await leaveService.GetMyLeaveBalancesAsync(cancellationToken));
    }

    [HttpGet("types")]
    public async Task<ActionResult<IReadOnlyList<LeaveTypeDto>>> GetLeaveTypes(CancellationToken cancellationToken)
    {
        return Ok(await leaveService.GetLeaveTypesAsync(cancellationToken));
    }

    [HttpPost]
    [Authorize(Roles = $"{RoleNames.Employee},{RoleNames.Manager},{RoleNames.Hr},{RoleNames.Admin}")]
    public async Task<ActionResult<LeaveRequestDto>> Apply([FromBody] ApplyLeaveRequestDto request, CancellationToken cancellationToken)
    {
        var created = await leaveService.ApplyLeaveAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetMine), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}/review")]
    [Authorize(Roles = $"{RoleNames.Manager},{RoleNames.Hr},{RoleNames.Admin}")]
    public async Task<ActionResult<LeaveRequestDto>> Review(int id, [FromBody] UpdateLeaveRequestStatusDto request, CancellationToken cancellationToken)
    {
        return Ok(await leaveService.ReviewLeaveAsync(id, request, cancellationToken));
    }
}
