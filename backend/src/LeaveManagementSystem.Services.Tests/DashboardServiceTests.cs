using LeaveManagementSystem.Business.Common.Exceptions;
using LeaveManagementSystem.Business.Interfaces;
using LeaveManagementSystem.Business.Interfaces.Repositories;
using LeaveManagementSystem.Business.Services;
using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Entities;
using Moq;
using Xunit;

namespace LeaveManagementSystem.Business.Tests;

public class DashboardServiceTests
{
    [Fact]
    public async Task GetSummaryAsync_ForEmployee_ReturnsEmployeeCounts()
    {
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        var employeeRepository = new Mock<IEmployeeRepository>(MockBehavior.Strict);
        var leaveRequestRepository = new Mock<ILeaveRequestRepository>(MockBehavior.Strict);
        var leaveBalanceRepository = new Mock<ILeaveBalanceRepository>(MockBehavior.Strict);

        currentUser.Setup(x => x.GetUserId()).Returns(7);
        currentUser.Setup(x => x.GetRole()).Returns(RoleNames.Employee);
        leaveBalanceRepository
            .Setup(x => x.GetByEmployeeAsync(7, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new LeaveBalance { RemainingDays = 10 },
                new LeaveBalance { RemainingDays = 7 }
            ]);
        leaveRequestRepository
            .Setup(x => x.GetByEmployeeAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new LeaveRequest(), new LeaveRequest(), new LeaveRequest()]);
        leaveRequestRepository.Setup(x => x.CountByStatusForEmployeeAsync(7, LeaveStatuses.Pending, It.IsAny<CancellationToken>())).ReturnsAsync(1);
        leaveRequestRepository.Setup(x => x.CountByStatusForEmployeeAsync(7, LeaveStatuses.Approved, It.IsAny<CancellationToken>())).ReturnsAsync(2);
        leaveRequestRepository.Setup(x => x.CountByStatusForEmployeeAsync(7, LeaveStatuses.Rejected, It.IsAny<CancellationToken>())).ReturnsAsync(0);

        var sut = new DashboardService(currentUser.Object, employeeRepository.Object, leaveRequestRepository.Object, leaveBalanceRepository.Object);

        var result = await sut.GetSummaryAsync();

        Assert.Equal(RoleNames.Employee, result.Role);
        Assert.Equal(17, result.AvailableBalanceDays);
        Assert.Equal(3, result.TotalLeaveRequests);
        Assert.Equal(1, result.PendingLeaves);
        Assert.Equal(2, result.ApprovedLeaves);
        Assert.Equal(0, result.RejectedLeaves);
    }

    [Fact]
    public async Task GetSummaryAsync_ForManager_ReturnsTeamCounts()
    {
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        var employeeRepository = new Mock<IEmployeeRepository>(MockBehavior.Strict);
        var leaveRequestRepository = new Mock<ILeaveRequestRepository>(MockBehavior.Strict);
        var leaveBalanceRepository = new Mock<ILeaveBalanceRepository>(MockBehavior.Strict);

        currentUser.Setup(x => x.GetUserId()).Returns(3);
        currentUser.Setup(x => x.GetRole()).Returns(RoleNames.Manager);
        leaveBalanceRepository
            .Setup(x => x.GetByEmployeeAsync(3, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([new LeaveBalance { RemainingDays = 8 }]);
        leaveRequestRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new LeaveRequest { Status = LeaveStatuses.Pending, Employee = new Employee { ManagerId = 3 } },
                new LeaveRequest { Status = LeaveStatuses.Approved, Employee = new Employee { ManagerId = 3 } },
                new LeaveRequest { Status = LeaveStatuses.Rejected, Employee = new Employee { ManagerId = 3 } },
                new LeaveRequest { Status = LeaveStatuses.Pending, Employee = new Employee { ManagerId = 99 } }
            ]);
        leaveRequestRepository
            .Setup(x => x.GetByEmployeeAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync([new LeaveRequest(), new LeaveRequest()]);

        var sut = new DashboardService(currentUser.Object, employeeRepository.Object, leaveRequestRepository.Object, leaveBalanceRepository.Object);

        var result = await sut.GetSummaryAsync();

        Assert.Equal(1, result.TeamPendingApprovals);
        Assert.Equal(1, result.PendingLeaves);
        Assert.Equal(1, result.ApprovedLeaves);
        Assert.Equal(1, result.RejectedLeaves);
        Assert.Equal(2, result.TotalLeaveRequests);
        Assert.Equal(8, result.AvailableBalanceDays);
    }

    [Fact]
    public async Task GetSummaryAsync_WithUnknownRole_ThrowsForbiddenException()
    {
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        var employeeRepository = new Mock<IEmployeeRepository>(MockBehavior.Strict);
        var leaveRequestRepository = new Mock<ILeaveRequestRepository>(MockBehavior.Strict);
        var leaveBalanceRepository = new Mock<ILeaveBalanceRepository>(MockBehavior.Strict);

        currentUser.Setup(x => x.GetUserId()).Returns(10);
        currentUser.Setup(x => x.GetRole()).Returns("Guest");
        leaveBalanceRepository
            .Setup(x => x.GetByEmployeeAsync(10, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<LeaveBalance>());

        var sut = new DashboardService(currentUser.Object, employeeRepository.Object, leaveRequestRepository.Object, leaveBalanceRepository.Object);

        var exception = await Assert.ThrowsAsync<ForbiddenException>(() => sut.GetSummaryAsync());

        Assert.Equal("You are not authorized to view organization-wide dashboard data.", exception.Message);
    }
}
