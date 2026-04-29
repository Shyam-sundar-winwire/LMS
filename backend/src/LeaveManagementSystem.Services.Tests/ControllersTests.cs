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
    public async Task Login_ReturnsOk_WhenValid()
    {
        var service = new Mock<IAuthService>();
        var response = new LoginResponseDto { Token = "token" };

        service.Setup(x => x.LoginAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(response);

        var controller = new AuthController(service.Object);

        var result = await controller.Login(new LoginRequestDto(), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(response, ok.Value);
    }

    [Fact]
    public async Task Login_ReturnsOkWithNull_WhenServiceReturnsNull()
    {
        var service = new Mock<IAuthService>();

        service.Setup(x => x.LoginAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((LoginResponseDto)null!);

        var controller = new AuthController(service.Object);

        var result = await controller.Login(new LoginRequestDto(), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Null(ok.Value);
    }

    [Fact]
    public async Task Login_ThrowsException()
    {
        var service = new Mock<IAuthService>();

        service.Setup(x => x.LoginAsync(It.IsAny<LoginRequestDto>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(new Exception("error"));

        var controller = new AuthController(service.Object);

        await Assert.ThrowsAsync<Exception>(() =>
            controller.Login(new LoginRequestDto(), CancellationToken.None));
    }

   

    [Fact]
    public async Task GetSummary_ReturnsOk()
    {
        var service = new Mock<IDashboardService>();
        var data = new DashboardSummaryDto();

        service.Setup(x => x.GetSummaryAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(data);

        var controller = new DashboardController(service.Object);

        var result = await controller.GetSummary(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(data, ok.Value);
    }

    [Fact]
    public async Task GetSummary_ReturnsNull()
    {
        var service = new Mock<IDashboardService>();

        service.Setup(x => x.GetSummaryAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync((DashboardSummaryDto)null!);

        var controller = new DashboardController(service.Object);

        var result = await controller.GetSummary(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Null(ok.Value);
    }

    

    [Fact]
    public async Task Apply_ReturnsCreated()
    {
        var service = new Mock<ILeaveService>();
        var leave = new LeaveRequestDto { Id = 1 };

        service.Setup(x => x.ApplyLeaveAsync(It.IsAny<ApplyLeaveRequestDto>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(leave);

        var controller = new LeaveRequestsController(service.Object);

        var result = await controller.Apply(new ApplyLeaveRequestDto(), CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(leave, created.Value);
    }

    [Fact]
    public async Task Apply_ThrowsException()
    {
        var service = new Mock<ILeaveService>();

        service.Setup(x => x.ApplyLeaveAsync(It.IsAny<ApplyLeaveRequestDto>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(new Exception());

        var controller = new LeaveRequestsController(service.Object);

        await Assert.ThrowsAsync<Exception>(() =>
            controller.Apply(new ApplyLeaveRequestDto(), CancellationToken.None));
    }

   

    [Fact]
    public async Task GetMine_ReturnsOk()
    {
        var service = new Mock<ILeaveService>();
        service.Setup(x => x.GetMyLeavesAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(new List<LeaveRequestDto> { new() });

        var controller = new LeaveRequestsController(service.Object);

        var result = await controller.GetMine(CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetMine_ReturnsEmpty()
    {
        var service = new Mock<ILeaveService>();
        service.Setup(x => x.GetMyLeavesAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(new List<LeaveRequestDto>());

        var controller = new LeaveRequestsController(service.Object);

        var result = await controller.GetMine(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var leaves = Assert.IsAssignableFrom<IEnumerable<LeaveRequestDto>>(ok.Value);
        Assert.Empty(leaves);
    }

    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        var service = new Mock<ILeaveService>();
        service.Setup(x => x.GetAllLeavesAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(new List<LeaveRequestDto> { new() });

        var controller = new LeaveRequestsController(service.Object);

        var result = await controller.GetAll(CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetApprovals_ReturnsOk()
    {
        var service = new Mock<ILeaveService>();
        service.Setup(x => x.GetPendingApprovalsAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(new List<LeaveRequestDto> { new() });

        var controller = new LeaveRequestsController(service.Object);

        var result = await controller.GetApprovals(CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetBalances_ReturnsOk()
    {
        var service = new Mock<ILeaveService>();
        service.Setup(x => x.GetMyLeaveBalancesAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(new List<LeaveBalanceDto> { new() });

        var controller = new LeaveRequestsController(service.Object);

        var result = await controller.GetBalances(CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetLeaveTypes_ReturnsOk()
    {
        var service = new Mock<ILeaveService>();
        service.Setup(x => x.GetLeaveTypesAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(new List<LeaveTypeDto> { new() });

        var controller = new LeaveRequestsController(service.Object);

        var result = await controller.GetLeaveTypes(CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

   

    [Fact]
    public async Task Review_ReturnsOk()
    {
        var service = new Mock<ILeaveService>();
        service.Setup(x => x.ReviewLeaveAsync(It.IsAny<int>(), It.IsAny<UpdateLeaveRequestStatusDto>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new LeaveRequestDto());

        var controller = new LeaveRequestsController(service.Object);

        var result = await controller.Review(1, new UpdateLeaveRequestStatusDto(), CancellationToken.None);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task Review_ReturnsOkWithNull_WhenServiceReturnsNull()
    {
        var service = new Mock<ILeaveService>();
        service.Setup(x => x.ReviewLeaveAsync(It.IsAny<int>(), It.IsAny<UpdateLeaveRequestStatusDto>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((LeaveRequestDto)null!);

        var controller = new LeaveRequestsController(service.Object);

        var result = await controller.Review(999, new UpdateLeaveRequestStatusDto(), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Null(ok.Value);
    }

    [Fact]
    public async Task Review_ThrowsException()
    {
        var service = new Mock<ILeaveService>();
        service.Setup(x => x.ReviewLeaveAsync(It.IsAny<int>(), It.IsAny<UpdateLeaveRequestStatusDto>(), It.IsAny<CancellationToken>()))
               .ThrowsAsync(new Exception());

        var controller = new LeaveRequestsController(service.Object);

        await Assert.ThrowsAsync<Exception>(() =>
            controller.Review(1, new UpdateLeaveRequestStatusDto(), CancellationToken.None));
    }
}
