using LeaveManagementSystem.Business.Common.Exceptions;
using LeaveManagementSystem.Business.DTOs.LeaveRequests;
using LeaveManagementSystem.Business.Interfaces;
using LeaveManagementSystem.Business.Interfaces.Repositories;
using LeaveManagementSystem.Business.Interfaces.Services;
using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Entities;

namespace LeaveManagementSystem.Business.Services;

public class LeaveService(
    ICurrentUserService currentUserService,
    IEmployeeRepository employeeRepository,
    ILeaveRequestRepository leaveRequestRepository,
    ILeaveBalanceRepository leaveBalanceRepository,
    ILeaveTypeRepository leaveTypeRepository,
    IUnitOfWork unitOfWork) : ILeaveService
{
    public async Task<LeaveRequestDto> ApplyLeaveAsync(ApplyLeaveRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request.EndDate < request.StartDate)
        {
            throw new ValidationException("End date must be on or after start date.");
        }

        var currentUserId = currentUserService.GetUserId();
        var leaveType = await leaveTypeRepository.GetByIdAsync(request.LeaveTypeId, cancellationToken)
            ?? throw new NotFoundException("Leave type not found.");

        var totalDays = request.EndDate.DayNumber - request.StartDate.DayNumber + 1;
        if (totalDays <= 0)
        {
            throw new ValidationException("Requested leave duration must be at least one day.");
        }

        var balance = await EnsureBalanceAsync(currentUserId, leaveType, DateTime.UtcNow.Year, cancellationToken);
        if (balance.RemainingDays < totalDays)
        {
            throw new ValidationException($"Insufficient balance. Remaining {leaveType.Name} days: {balance.RemainingDays}.");
        }

        var leaveRequest = new LeaveRequest
        {
            EmployeeId = currentUserId,
            LeaveTypeId = leaveType.Id,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = LeaveStatuses.Pending,
            Reason = request.Reason.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        await leaveRequestRepository.AddAsync(leaveRequest, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var saved = await leaveRequestRepository.GetByIdAsync(leaveRequest.Id, cancellationToken)
            ?? throw new NotFoundException("Unable to load the created leave request.");

        return MapLeaveRequest(saved);
    }

    public async Task<IReadOnlyList<LeaveRequestDto>> GetMyLeavesAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserService.GetUserId();
        var leaveRequests = await leaveRequestRepository.GetByEmployeeAsync(currentUserId, cancellationToken);
        return leaveRequests.Select(MapLeaveRequest).ToList();
    }

    public async Task<IReadOnlyList<LeaveRequestDto>> GetPendingApprovalsAsync(CancellationToken cancellationToken = default)
    {
        var role = currentUserService.GetRole();
        var currentUserId = currentUserService.GetUserId();
        if (role is not (RoleNames.Manager or RoleNames.Hr or RoleNames.Admin))
        {
            throw new ForbiddenException("You are not authorized to view approvals.");
        }

        var items = role == RoleNames.Manager
            ? await leaveRequestRepository.GetPendingForManagerAsync(currentUserId, cancellationToken)
            : (await leaveRequestRepository.GetAllAsync(cancellationToken)).Where(x => x.Status == LeaveStatuses.Pending).ToList();

        return items.Select(MapLeaveRequest).ToList();
    }

    public async Task<IReadOnlyList<LeaveRequestDto>> GetAllLeavesAsync(CancellationToken cancellationToken = default)
    {
        var role = currentUserService.GetRole();
        if (role is not (RoleNames.Hr or RoleNames.Admin))
        {
            throw new ForbiddenException("Only HR and Admin users can view all leave requests.");
        }

        var leaveRequests = await leaveRequestRepository.GetAllAsync(cancellationToken);
        return leaveRequests.Select(MapLeaveRequest).ToList();
    }

    public async Task<IReadOnlyList<LeaveBalanceDto>> GetMyLeaveBalancesAsync(CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserService.GetUserId();
        var leaveTypes = await leaveTypeRepository.GetAllAsync(cancellationToken);
        var year = DateTime.UtcNow.Year;
        var balances = new List<LeaveBalanceDto>();

        foreach (var leaveType in leaveTypes)
        {
            var balance = await EnsureBalanceAsync(currentUserId, leaveType, year, cancellationToken);
            balances.Add(new LeaveBalanceDto
            {
                EmployeeId = currentUserId,
                LeaveTypeId = leaveType.Id,
                LeaveTypeName = leaveType.Name,
                Year = year,
                RemainingDays = balance.RemainingDays
            });
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return balances.OrderBy(x => x.LeaveTypeName).ToList();
    }

    public async Task<IReadOnlyList<LeaveTypeDto>> GetLeaveTypesAsync(CancellationToken cancellationToken = default)
    {
        var leaveTypes = await leaveTypeRepository.GetAllAsync(cancellationToken);
        return leaveTypes.Select(x => new LeaveTypeDto
        {
            Id = x.Id,
            Name = x.Name,
            DefaultDays = x.DefaultDays
        }).ToList();
    }

    public async Task<LeaveRequestDto> ReviewLeaveAsync(int leaveRequestId, UpdateLeaveRequestStatusDto request, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserService.GetUserId();
        var role = currentUserService.GetRole();
        var leaveRequest = await leaveRequestRepository.GetByIdAsync(leaveRequestId, cancellationToken)
            ?? throw new NotFoundException("Leave request not found.");

        if (leaveRequest.Status != LeaveStatuses.Pending)
        {
            throw new ValidationException("Only pending leave requests can be reviewed.");
        }

        var owner = await employeeRepository.GetByIdAsync(leaveRequest.EmployeeId, cancellationToken)
            ?? throw new NotFoundException("Employee not found.");

        var isPrivilegedReviewer = role is RoleNames.Hr or RoleNames.Admin;
        var isManagerReviewer = role == RoleNames.Manager && owner.ManagerId == currentUserId;
        if (!isPrivilegedReviewer && !isManagerReviewer)
        {
            throw new ForbiddenException("You are not allowed to review this leave request.");
        }

        leaveRequest.Status = request.Approve ? LeaveStatuses.Approved : LeaveStatuses.Rejected;
        leaveRequest.ReviewedAtUtc = DateTime.UtcNow;
        leaveRequest.ReviewedByEmployeeId = currentUserId;
        leaveRequest.ManagerComment = request.Comment?.Trim();

        if (request.Approve)
        {
            var leaveType = await leaveTypeRepository.GetByIdAsync(leaveRequest.LeaveTypeId, cancellationToken)
                ?? throw new NotFoundException("Leave type not found.");
            var balance = await EnsureBalanceAsync(owner.Id, leaveType, leaveRequest.StartDate.Year, cancellationToken);
            var requestedDays = leaveRequest.EndDate.DayNumber - leaveRequest.StartDate.DayNumber + 1;

            if (balance.RemainingDays < requestedDays)
            {
                throw new ValidationException($"Insufficient balance to approve this request. Remaining {leaveType.Name} days: {balance.RemainingDays}.");
            }

            balance.RemainingDays -= requestedDays;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return MapLeaveRequest(leaveRequest);
    }

    private async Task<LeaveBalance> EnsureBalanceAsync(int employeeId, LeaveType leaveType, int year, CancellationToken cancellationToken)
    {
        var balance = await leaveBalanceRepository.GetAsync(employeeId, leaveType.Id, year, cancellationToken);
        if (balance is not null)
        {
            return balance;
        }

        balance = new LeaveBalance
        {
            EmployeeId = employeeId,
            LeaveTypeId = leaveType.Id,
            Year = year,
            RemainingDays = leaveType.DefaultDays
        };

        await leaveBalanceRepository.AddAsync(balance, cancellationToken);
        return balance;
    }

    private static LeaveRequestDto MapLeaveRequest(LeaveRequest entity)
    {
        return new LeaveRequestDto
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            EmployeeName = entity.Employee?.FullName ?? string.Empty,
            EmployeeEmail = entity.Employee?.Email ?? string.Empty,
            LeaveTypeId = entity.LeaveTypeId,
            LeaveTypeName = entity.LeaveType?.Name ?? string.Empty,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            DaysRequested = entity.EndDate.DayNumber - entity.StartDate.DayNumber + 1,
            Status = entity.Status,
            Reason = entity.Reason,
            CreatedAtUtc = entity.CreatedAtUtc,
            ReviewedAtUtc = entity.ReviewedAtUtc,
            ManagerComment = entity.ManagerComment
        };
    }
}
