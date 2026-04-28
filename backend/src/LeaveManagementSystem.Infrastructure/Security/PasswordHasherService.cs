using LeaveManagementSystem.Business.Common.Exceptions;
using LeaveManagementSystem.Business.Interfaces.Security;
using LeaveManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LeaveManagementSystem.Infrastructure.Security;

public class PasswordHasherService : IPasswordHasher
{
    private readonly PasswordHasher<Employee> _passwordHasher = new();

    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ValidationException("Password is required.");
        }

        return _passwordHasher.HashPassword(new Employee(), password);
    }

    public bool Verify(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrWhiteSpace(providedPassword))
        {
            return false;
        }

        var result = _passwordHasher.VerifyHashedPassword(new Employee(), hashedPassword, providedPassword);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
