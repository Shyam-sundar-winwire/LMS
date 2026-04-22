using LeaveManagementSystem.Business.Interfaces.Security;
using LeaveManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LeaveManagementSystem.Infrastructure.Security;

public class PasswordHasherService : IPasswordHasher
{
    private readonly PasswordHasher<Employee> _passwordHasher = new();

    public string Hash(string password)
    {
        return _passwordHasher.HashPassword(new Employee(), password);
    }

    public bool Verify(string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(new Employee(), hashedPassword, providedPassword);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
