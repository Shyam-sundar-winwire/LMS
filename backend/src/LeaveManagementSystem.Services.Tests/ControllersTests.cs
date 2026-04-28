using LeaveManagementSystem.API.Controllers;
using LeaveManagementSystem.Business.DTOs.Auth;
using LeaveManagementSystem.Business.DTOs.Dashboard;
using LeaveManagementSystem.Business.DTOs.LeaveRequests;
using LeaveManagementSystem.Business.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LeaveManagementSystem.Business.Tests;

public class ControllersTests
{
    [Fact]
    public async Task AuthController_Login_ReturnsOk()
    {
        var authService = new Mock<IAuthService>(MockBehavior.Strict);
        var response = new LoginResponseDto { Token = "token", User = new UserProfileDto { Id = 1, Email = "a@b.com", FullName = "A", Role = "Employee" } };
        authService.Setup(x => x.LoginAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var controller = new AuthController(authService.Object);
        var result = await controller.Login(new LoginRequestDto { Email = "a@b.com", Password = "Password@123" }, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(response, ok.Value);
    }

    [Fact]
    public async Task DashboardController_GetSummary_ReturnsOk()
    {
        var dashboardService = new Mock<IDashboardService>(MockBehavior.Strict);
        var summary = new DashboardSummaryDto { Role = "Employee", AvailableBalanceDays = 10 };
        dashboardService.Setup(x => x.GetSummaryAsync(It.IsAny<CancellationToken>())).ReturnsAsync(summary);

        var controller = new DashboardController(dashboardService.Object);
        var result = await controller.GetSummary(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(summary, ok.Value);
    }

    [Fact]
    public async Task LeaveRequestsController_Apply_ReturnsCreated()
    {
        var leaveService = new Mock<ILeaveService>(MockBehavior.Strict);
        var createdLeave = new LeaveRequestDto { Id = 5, EmployeeId = 1, LeaveTypeId = 1, Status = "Pending", Reason = "Trip" };
        leaveService.Setup(x => x.ApplyLeaveAsync(It.IsAny<ApplyLeaveRequestDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdLeave);

        var controller = new LeaveRequestsController(leaveService.Object);
        var result = await controller.Apply(new ApplyLeaveRequestDto { LeaveTypeId = 1, StartDate = new DateOnly(2026, 5, 1), EndDate = new DateOnly(2026, 5, 2), Reason = "Trip" }, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(LeaveRequestsController.GetMine), created.ActionName);
        Assert.Same(createdLeave, created.Value);
    }

    [Fact]
    public async Task LeaveRequestsController_GetMine_ReturnsOk()
    {
        var leaveService = new Mock<ILeaveService>(MockBehavior.Strict);
        leaveService.Setup(x => x.GetMyLeavesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new LeaveRequestDto { Id = 1 }]);

        var controller = new LeaveRequestsController(leaveService.Object);
        var result = await controller.GetMine(CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task LeaveRequestsController_GetAll_ReturnsOk()
    {
        var leaveService = new Mock<ILeaveService>(MockBehavior.Strict);
        leaveService.Setup(x => x.GetAllLeavesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new LeaveRequestDto { Id = 1 }]);

        var controller = new LeaveRequestsController(leaveService.Object);
        var result = await controller.GetAll(CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task LeaveRequestsController_GetApprovals_ReturnsOk()
    {
        var leaveService = new Mock<ILeaveService>(MockBehavior.Strict);
        leaveService.Setup(x => x.GetPendingApprovalsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new LeaveRequestDto { Id = 1 }]);

        var controller = new LeaveRequestsController(leaveService.Object);
        var result = await controller.GetApprovals(CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task LeaveRequestsController_GetBalances_ReturnsOk()
    {
        var leaveService = new Mock<ILeaveService>(MockBehavior.Strict);
        leaveService.Setup(x => x.GetMyLeaveBalancesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new LeaveBalanceDto { EmployeeId = 1, LeaveTypeId = 1 }]);

        var controller = new LeaveRequestsController(leaveService.Object);
        var result = await controller.GetBalances(CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task LeaveRequestsController_GetLeaveTypes_ReturnsOk()
    {
        var leaveService = new Mock<ILeaveService>(MockBehavior.Strict);
        leaveService.Setup(x => x.GetLeaveTypesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new LeaveTypeDto { Id = 1, Name = "Annual Leave", DefaultDays = 18 }]);

        var controller = new LeaveRequestsController(leaveService.Object);
        var result = await controller.GetLeaveTypes(CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task LeaveRequestsController_Review_ReturnsOk()
    {
        var leaveService = new Mock<ILeaveService>(MockBehavior.Strict);
        leaveService.Setup(x => x.ReviewLeaveAsync(5, It.IsAny<UpdateLeaveRequestStatusDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeaveRequestDto { Id = 5, Status = "Approved" });

        var controller = new LeaveRequestsController(leaveService.Object);
        var result = await controller.Review(5, new UpdateLeaveRequestStatusDto { Approve = true }, CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

}
