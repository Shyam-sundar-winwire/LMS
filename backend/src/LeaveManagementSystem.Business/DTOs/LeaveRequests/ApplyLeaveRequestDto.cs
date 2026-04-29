using System.ComponentModel.DataAnnotations;

namespace LeaveManagementSystem.Business.DTOs.LeaveRequests;

public class ApplyLeaveRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Please select a valid leave type.")]
    public int LeaveTypeId { get; set; }

    [Required(ErrorMessage = "Start date is required.")]
    [DateInFuture(ErrorMessage = "Start date must be today or in the future.")]
    public DateOnly StartDate { get; set; }

    [Required(ErrorMessage = "End date is required.")]
    [DateGreaterThan("StartDate", ErrorMessage = "End date must be on or after start date.")]
    public DateOnly EndDate { get; set; }

    [Required(ErrorMessage = "Reason is required.")]
    [StringLength(1000, MinimumLength = 3, ErrorMessage = "Reason must be between 3 and 1000 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?-]+$", ErrorMessage = "Reason contains invalid characters.")]
    public string Reason { get; set; } = string.Empty;
}

public class DateInFutureAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateOnly date)
        {
            return date >= DateOnly.FromDateTime(DateTime.Today) 
                ? ValidationResult.Success 
                : new ValidationResult(ErrorMessage);
        }
        return new ValidationResult("Invalid date format.");
    }
}

public class DateGreaterThanAttribute : ValidationAttribute
{
    private readonly string _comparisonProperty;

    public DateGreaterThanAttribute(string comparisonProperty)
    {
        _comparisonProperty = comparisonProperty;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateOnly endDate)
        {
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property?.GetValue(validationContext.ObjectInstance) is DateOnly startDate)
            {
                return endDate >= startDate 
                    ? ValidationResult.Success 
                    : new ValidationResult(ErrorMessage);
            }
        }
        return new ValidationResult("Invalid date comparison.");
    }
}
