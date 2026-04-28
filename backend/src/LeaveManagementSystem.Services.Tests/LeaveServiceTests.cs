using LeaveManagementSystem.Business.Common.Exceptions;
using LeaveManagementSystem.Business.DTOs.LeaveRequests;
using LeaveManagementSystem.Business.Interfaces;
using LeaveManagementSystem.Business.Interfaces.Repositories;
using LeaveManagementSystem.Business.Services;
using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Entities;
using Moq;
using Xunit;

namespace LeaveManagementSystem.Business.Tests;

public class LeaveServiceTests
{
    [Fact]
    public async Task ApplyLeaveAsync_ValidRequest_CreatesLeave_WithCorrectBusinessRules()
    {
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.Setup(x => x.GetUserId()).Returns(1);

        var leaveRepo = new Mock<ILeaveRequestRepository>();
        var balanceRepo = new Mock<ILeaveBalanceRepository>();
        var typeRepo = new Mock<ILeaveTypeRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        typeRepo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeaveType { Id = 1, Name = "Annual Leave", DefaultDays = 10 });

        balanceRepo.Setup(x => x.GetAsync(1, 1, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeaveBalance { EmployeeId = 1, LeaveTypeId = 1, Year = 2026, RemainingDays = 10 });

        LeaveRequest? savedLeave = null;

        leaveRepo.Setup(x => x.AddAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()))
            .Callback<LeaveRequest, CancellationToken>((lr, _) =>
            {
                lr.Id = 1;
                lr.LeaveType = new LeaveType { Id = 1, Name = "Annual Leave", DefaultDays = 10 };
                savedLeave = lr;
            })
            .Returns(Task.CompletedTask);

        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        leaveRepo.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => savedLeave);

        var service = new LeaveService(
            currentUser.Object,
            Mock.Of<IEmployeeRepository>(),
            leaveRepo.Object,
            balanceRepo.Object,
            typeRepo.Object,
            unitOfWork.Object);

        var request = new ApplyLeaveRequestDto
        {
            LeaveTypeId = 1,
            StartDate = new DateOnly(2026, 5, 10),
            EndDate = new DateOnly(2026, 5, 12),
            Reason = "Family Trip"
        };

        await service.ApplyLeaveAsync(request);

        Assert.NotNull(savedLeave);
        Assert.Equal(1, savedLeave!.EmployeeId);
        Assert.Equal(1, savedLeave.LeaveTypeId);
        Assert.Equal("Family Trip", savedLeave.Reason);
        Assert.Equal(LeaveStatuses.Pending, savedLeave.Status);
        Assert.Equal(3, (savedLeave.EndDate.DayNumber - savedLeave.StartDate.DayNumber) + 1);
        Assert.Null(savedLeave.ReviewedAtUtc);
        Assert.Null(savedLeave.ReviewedByEmployeeId);

        leaveRepo.Verify(x => x.AddAsync(It.IsAny<LeaveRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApplyLeaveAsync_WithBlankReason_ThrowsValidationException()
    {
        var service = BuildLeaveService();

        var exception = await Assert.ThrowsAsync<ValidationException>(() => service.ApplyLeaveAsync(new ApplyLeaveRequestDto
        {
            LeaveTypeId = 1,
            StartDate = new DateOnly(2026, 5, 10),
            EndDate = new DateOnly(2026, 5, 12),
            Reason = "   "
        }));

        Assert.Equal("Reason is required.", exception.Message);
    }

    [Fact]
    public async Task ApplyLeaveAsync_WithDefaultDates_ThrowsValidationException()
    {
        var service = BuildLeaveService();

        var exception = await Assert.ThrowsAsync<ValidationException>(() => service.ApplyLeaveAsync(new ApplyLeaveRequestDto
        {
            LeaveTypeId = 1,
            Reason = "Trip"
        }));

        Assert.Equal("Start date and end date are required.", exception.Message);
    }

    [Fact]
    public async Task ApplyLeaveAsync_WithInsufficientBalance_ThrowsValidationException()
    {
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        var leaveBalanceRepository = new Mock<ILeaveBalanceRepository>(MockBehavior.Strict);
        var leaveTypeRepository = new Mock<ILeaveTypeRepository>(MockBehavior.Strict);

        currentUser.Setup(x => x.GetUserId()).Returns(1);
        leaveTypeRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeaveType { Id = 1, Name = "Annual Leave", DefaultDays = 10 });
        leaveBalanceRepository.Setup(x => x.GetAsync(1, 1, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeaveBalance { RemainingDays = 1 });

        var service = new LeaveService(
            currentUser.Object,
            Mock.Of<IEmployeeRepository>(),
            Mock.Of<ILeaveRequestRepository>(),
            leaveBalanceRepository.Object,
            leaveTypeRepository.Object,
            Mock.Of<IUnitOfWork>());

        var exception = await Assert.ThrowsAsync<ValidationException>(() => service.ApplyLeaveAsync(new ApplyLeaveRequestDto
        {
            LeaveTypeId = 1,
            StartDate = new DateOnly(2026, 5, 10),
            EndDate = new DateOnly(2026, 5, 12),
            Reason = "Trip"
        }));

        Assert.Contains("Insufficient balance.", exception.Message);
    }

    [Fact]
    public async Task ReviewLeaveAsync_WithInvalidId_ThrowsValidationException()
    {
        var service = BuildLeaveService();

        var exception = await Assert.ThrowsAsync<ValidationException>(() => service.ReviewLeaveAsync(0, new UpdateLeaveRequestStatusDto()));

        Assert.Equal("A valid leave request id is required.", exception.Message);
    }

    [Fact]
    public async Task ReviewLeaveAsync_WithLongComment_ThrowsValidationException()
    {
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        var employeeRepository = new Mock<IEmployeeRepository>(MockBehavior.Strict);
        var leaveRequestRepository = new Mock<ILeaveRequestRepository>(MockBehavior.Strict);

        currentUser.Setup(x => x.GetUserId()).Returns(3);
        currentUser.Setup(x => x.GetRole()).Returns(RoleNames.Manager);
        leaveRequestRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeaveRequest
            {
                Id = 1,
                EmployeeId = 7,
                LeaveTypeId = 1,
                Status = LeaveStatuses.Pending,
                StartDate = new DateOnly(2026, 5, 10),
                EndDate = new DateOnly(2026, 5, 11)
            });

        var service = new LeaveService(
            currentUser.Object,
            employeeRepository.Object,
            leaveRequestRepository.Object,
            Mock.Of<ILeaveBalanceRepository>(),
            Mock.Of<ILeaveTypeRepository>(),
            Mock.Of<IUnitOfWork>());

        var exception = await Assert.ThrowsAsync<ValidationException>(() => service.ReviewLeaveAsync(1, new UpdateLeaveRequestStatusDto
        {
            Approve = true,
            Comment = new string('a', 501)
        }));

        Assert.Equal("Review comment cannot exceed 500 characters.", exception.Message);
    }

    [Fact]
    public async Task GetPendingApprovalsAsync_ForUnauthorizedRole_ThrowsForbiddenException()
    {
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUser.Setup(x => x.GetRole()).Returns(RoleNames.Employee);
        currentUser.Setup(x => x.GetUserId()).Returns(1);

        var service = new LeaveService(
            currentUser.Object,
            Mock.Of<IEmployeeRepository>(),
            Mock.Of<ILeaveRequestRepository>(),
            Mock.Of<ILeaveBalanceRepository>(),
            Mock.Of<ILeaveTypeRepository>(),
            Mock.Of<IUnitOfWork>());

        var exception = await Assert.ThrowsAsync<ForbiddenException>(() => service.GetPendingApprovalsAsync());

        Assert.Equal("You are not authorized to view approvals.", exception.Message);
    }

    [Fact]
    public async Task GetMyLeavesAsync_ReturnsMappedLeaveRequests()
    {
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        var leaveRequestRepository = new Mock<ILeaveRequestRepository>(MockBehavior.Strict);
        currentUser.Setup(x => x.GetUserId()).Returns(1);
        leaveRequestRepository.Setup(x => x.GetByEmployeeAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new LeaveRequest
                {
                    Id = 1,
                    EmployeeId = 1,
                    Employee = new Employee { FullName = "Ravi", Email = "ravi@demo.com" },
                    LeaveTypeId = 1,
                    LeaveType = new LeaveType { Name = "Annual Leave" },
                    Status = LeaveStatuses.Pending,
                    Reason = "Trip",
                    StartDate = new DateOnly(2026, 5, 10),
                    EndDate = new DateOnly(2026, 5, 12),
                    CreatedAtUtc = DateTime.UtcNow
                }
            ]);

        var service = new LeaveService(currentUser.Object, Mock.Of<IEmployeeRepository>(), leaveRequestRepository.Object, Mock.Of<ILeaveBalanceRepository>(), Mock.Of<ILeaveTypeRepository>(), Mock.Of<IUnitOfWork>());

        var result = await service.GetMyLeavesAsync();

        Assert.Single(result);
        Assert.Equal("Ravi", result[0].EmployeeName);
    }

    [Fact]
    public async Task GetAllLeavesAsync_ForAdmin_ReturnsRequests()
    {
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        var leaveRequestRepository = new Mock<ILeaveRequestRepository>(MockBehavior.Strict);
        currentUser.Setup(x => x.GetRole()).Returns(RoleNames.Admin);
        leaveRequestRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new LeaveRequest
                {
                    Id = 1,
                    EmployeeId = 1,
                    Employee = new Employee { FullName = "Ravi", Email = "ravi@demo.com" },
                    LeaveTypeId = 1,
                    LeaveType = new LeaveType { Name = "Annual Leave" },
                    Status = LeaveStatuses.Pending,
                    Reason = "Trip",
                    StartDate = new DateOnly(2026, 5, 10),
                    EndDate = new DateOnly(2026, 5, 12),
                    CreatedAtUtc = DateTime.UtcNow
                }
            ]);

        var service = new LeaveService(currentUser.Object, Mock.Of<IEmployeeRepository>(), leaveRequestRepository.Object, Mock.Of<ILeaveBalanceRepository>(), Mock.Of<ILeaveTypeRepository>(), Mock.Of<IUnitOfWork>());

        var result = await service.GetAllLeavesAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task GetMyLeaveBalancesAsync_CreatesMissingBalancesAndReturnsOrderedList()
    {
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        var leaveBalanceRepository = new Mock<ILeaveBalanceRepository>(MockBehavior.Strict);
        var leaveTypeRepository = new Mock<ILeaveTypeRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);
        currentUser.Setup(x => x.GetUserId()).Returns(2);
        leaveTypeRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new LeaveType { Id = 2, Name = "Sick Leave", DefaultDays = 10 },
                new LeaveType { Id = 1, Name = "Annual Leave", DefaultDays = 18 }
            ]);
        leaveBalanceRepository.Setup(x => x.GetAsync(2, 2, It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((LeaveBalance?)null);
        leaveBalanceRepository.Setup(x => x.GetAsync(2, 1, It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new LeaveBalance { EmployeeId = 2, LeaveTypeId = 1, Year = DateTime.UtcNow.Year, RemainingDays = 15 });
        leaveBalanceRepository.Setup(x => x.AddAsync(It.IsAny<LeaveBalance>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = new LeaveService(currentUser.Object, Mock.Of<IEmployeeRepository>(), Mock.Of<ILeaveRequestRepository>(), leaveBalanceRepository.Object, leaveTypeRepository.Object, unitOfWork.Object);

        var result = await service.GetMyLeaveBalancesAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal(["Annual Leave", "Sick Leave"], result.Select(x => x.LeaveTypeName).ToArray());
        leaveBalanceRepository.Verify(x => x.AddAsync(It.Is<LeaveBalance>(lb => lb.LeaveTypeId == 2 && lb.RemainingDays == 10), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetLeaveTypesAsync_ReturnsOrderedTypes()
    {
        var leaveTypeRepository = new Mock<ILeaveTypeRepository>(MockBehavior.Strict);
        leaveTypeRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new LeaveType { Id = 1, Name = "Annual Leave", DefaultDays = 18 },
                new LeaveType { Id = 2, Name = "Sick Leave", DefaultDays = 10 }
            ]);

        var service = new LeaveService(Mock.Of<ICurrentUserService>(), Mock.Of<IEmployeeRepository>(), Mock.Of<ILeaveRequestRepository>(), Mock.Of<ILeaveBalanceRepository>(), leaveTypeRepository.Object, Mock.Of<IUnitOfWork>());

        var result = await service.GetLeaveTypesAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Annual Leave", result[0].Name);
    }

    [Fact]
    public async Task GetPendingApprovalsAsync_ForManager_ReturnsPendingItems()
    {
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        var leaveRequestRepository = new Mock<ILeaveRequestRepository>(MockBehavior.Strict);
        currentUser.Setup(x => x.GetRole()).Returns(RoleNames.Manager);
        currentUser.Setup(x => x.GetUserId()).Returns(3);
        leaveRequestRepository.Setup(x => x.GetPendingForManagerAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new LeaveRequest
                {
                    Id = 1,
                    EmployeeId = 2,
                    Employee = new Employee { FullName = "A", Email = "a@demo.com" },
                    LeaveTypeId = 1,
                    LeaveType = new LeaveType { Name = "Annual Leave" },
                    Status = LeaveStatuses.Pending,
                    Reason = "Trip",
                    StartDate = new DateOnly(2026, 5, 1),
                    EndDate = new DateOnly(2026, 5, 2),
                    CreatedAtUtc = DateTime.UtcNow
                }
            ]);

        var service = new LeaveService(currentUser.Object, Mock.Of<IEmployeeRepository>(), leaveRequestRepository.Object, Mock.Of<ILeaveBalanceRepository>(), Mock.Of<ILeaveTypeRepository>(), Mock.Of<IUnitOfWork>());

        var result = await service.GetPendingApprovalsAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task ReviewLeaveAsync_ForUnauthorizedReviewer_ThrowsForbiddenException()
    {
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        var employeeRepository = new Mock<IEmployeeRepository>(MockBehavior.Strict);
        var leaveRequestRepository = new Mock<ILeaveRequestRepository>(MockBehavior.Strict);
        currentUser.Setup(x => x.GetUserId()).Returns(5);
        currentUser.Setup(x => x.GetRole()).Returns(RoleNames.Manager);
        leaveRequestRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeaveRequest
            {
                Id = 1,
                EmployeeId = 7,
                LeaveTypeId = 1,
                Status = LeaveStatuses.Pending,
                StartDate = new DateOnly(2026, 5, 10),
                EndDate = new DateOnly(2026, 5, 11)
            });
        employeeRepository.Setup(x => x.GetByIdAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Employee { Id = 7, ManagerId = 99 });

        var service = new LeaveService(currentUser.Object, employeeRepository.Object, leaveRequestRepository.Object, Mock.Of<ILeaveBalanceRepository>(), Mock.Of<ILeaveTypeRepository>(), Mock.Of<IUnitOfWork>());

        var exception = await Assert.ThrowsAsync<ForbiddenException>(() => service.ReviewLeaveAsync(1, new UpdateLeaveRequestStatusDto { Approve = true }));

        Assert.Equal("You are not allowed to review this leave request.", exception.Message);
    }

    [Fact]
    public async Task ReviewLeaveAsync_WhenApproved_DeductsBalanceAndReturnsUpdatedRequest()
    {
        var currentUser = new Mock<ICurrentUserService>(MockBehavior.Strict);
        var employeeRepository = new Mock<IEmployeeRepository>(MockBehavior.Strict);
        var leaveRequestRepository = new Mock<ILeaveRequestRepository>(MockBehavior.Strict);
        var leaveBalanceRepository = new Mock<ILeaveBalanceRepository>(MockBehavior.Strict);
        var leaveTypeRepository = new Mock<ILeaveTypeRepository>(MockBehavior.Strict);
        var unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Strict);

        var leaveRequest = new LeaveRequest
        {
            Id = 1,
            EmployeeId = 7,
            Employee = new Employee { Id = 7, FullName = "Ravi", Email = "ravi@demo.com" },
            LeaveTypeId = 1,
            LeaveType = new LeaveType { Id = 1, Name = "Annual Leave", DefaultDays = 18 },
            Status = LeaveStatuses.Pending,
            Reason = "Trip",
            StartDate = new DateOnly(2026, 5, 10),
            EndDate = new DateOnly(2026, 5, 12),
            CreatedAtUtc = DateTime.UtcNow
        };
        var owner = new Employee { Id = 7, ManagerId = 3 };
        var leaveType = new LeaveType { Id = 1, Name = "Annual Leave", DefaultDays = 18 };
        var balance = new LeaveBalance { EmployeeId = 7, LeaveTypeId = 1, Year = 2026, RemainingDays = 10 };

        currentUser.Setup(x => x.GetUserId()).Returns(3);
        currentUser.Setup(x => x.GetRole()).Returns(RoleNames.Manager);
        leaveRequestRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(leaveRequest);
        employeeRepository.Setup(x => x.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(owner);
        leaveTypeRepository.Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(leaveType);
        leaveBalanceRepository.Setup(x => x.GetAsync(7, 1, 2026, It.IsAny<CancellationToken>())).ReturnsAsync(balance);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = new LeaveService(currentUser.Object, employeeRepository.Object, leaveRequestRepository.Object, leaveBalanceRepository.Object, leaveTypeRepository.Object, unitOfWork.Object);

        var result = await service.ReviewLeaveAsync(1, new UpdateLeaveRequestStatusDto { Approve = true, Comment = "Approved" });

        Assert.Equal(LeaveStatuses.Approved, result.Status);
        Assert.Equal(7, balance.RemainingDays);
        Assert.Equal("Approved", result.ManagerComment);
    }

    private static LeaveService BuildLeaveService()
    {
        return new LeaveService(
            Mock.Of<ICurrentUserService>(),
            Mock.Of<IEmployeeRepository>(),
            Mock.Of<ILeaveRequestRepository>(),
            Mock.Of<ILeaveBalanceRepository>(),
            Mock.Of<ILeaveTypeRepository>(),
            Mock.Of<IUnitOfWork>());
    }
}
