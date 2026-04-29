using System.ComponentModel.DataAnnotations;
using LeaveManagementSystem.Business.DTOs.LeaveRequests;
using Xunit;

namespace LeaveManagementSystem.Business.Tests;

public class ValidationAttributeTests
{
    [Fact]
    public void ApplyLeaveRequestDto_ValidDates_PassesCustomValidation()
    {
        var request = new ApplyLeaveRequestDto
        {
            LeaveTypeId = 1,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            Reason = "Family trip"
        };

        var results = Validate(request);

        Assert.Empty(results);
    }

    [Fact]
    public void ApplyLeaveRequestDto_PastStartDate_FailsFutureValidation()
    {
        var request = new ApplyLeaveRequestDto
        {
            LeaveTypeId = 1,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            Reason = "Family trip"
        };

        var results = Validate(request);

        Assert.Contains(results, x => x.ErrorMessage == "Start date must be today or in the future.");
    }

    [Fact]
    public void ApplyLeaveRequestDto_EndBeforeStart_FailsDateComparisonValidation()
    {
        var request = new ApplyLeaveRequestDto
        {
            LeaveTypeId = 1,
            StartDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2)),
            Reason = "Family trip"
        };

        var results = Validate(request);

        Assert.Contains(results, x => x.ErrorMessage == "End date must be on or after start date.");
    }

    [Fact]
    public void CustomDateAttributes_InvalidValueTypes_ReturnInvalidMessages()
    {
        var futureAttribute = new DateInFutureAttribute();
        var comparisonAttribute = new DateGreaterThanAttribute("MissingDate");
        var context = new ValidationContext(new { MissingDate = "not a date" });

        var futureResult = futureAttribute.GetValidationResult("not a date", context);
        var comparisonResult = comparisonAttribute.GetValidationResult("not a date", context);

        Assert.Equal("Invalid date format.", futureResult!.ErrorMessage);
        Assert.Equal("Invalid date comparison.", comparisonResult!.ErrorMessage);
    }

    private static List<ValidationResult> Validate(ApplyLeaveRequestDto request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true);
        return results;
    }
}
