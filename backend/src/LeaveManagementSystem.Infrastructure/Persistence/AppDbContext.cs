using LeaveManagementSystem.Domain.Common;
using LeaveManagementSystem.Domain.Entities;
using LeaveManagementSystem.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<LeaveRequest> LeaveRequests => Set<LeaveRequest>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeaveBalance> LeaveBalances => Set<LeaveBalance>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(50).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Email).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Password).HasMaxLength(500).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(150).IsRequired();
            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasOne(x => x.Role)
                .WithMany(x => x.Employees)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.Manager)
                .WithMany(x => x.DirectReports)
                .HasForeignKey(x => x.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.ToTable("LeaveTypes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.ToTable("LeaveRequests");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Status).HasMaxLength(30).IsRequired();
            entity.Property(x => x.Reason).HasMaxLength(1000).IsRequired();
            entity.Property(x => x.ManagerComment).HasMaxLength(500);
            entity.HasOne(x => x.Employee)
                .WithMany(x => x.LeaveRequests)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.LeaveType)
                .WithMany(x => x.LeaveRequests)
                .HasForeignKey(x => x.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(x => x.ReviewedByEmployee)
                .WithMany()
                .HasForeignKey(x => x.ReviewedByEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // Database constraints for data integrity
            entity.ToTable(table => table.HasCheckConstraint("CK_LeaveRequest_ValidDates", "EndDate >= StartDate"));
            entity.ToTable(table => table.HasCheckConstraint("CK_LeaveRequest_FutureDates", "StartDate >= CAST(GETDATE() AS DATE)"));
            entity.ToTable(table => table.HasCheckConstraint("CK_LeaveRequest_ValidStatus", "Status IN ('Pending', 'Approved', 'Rejected')"));
        });

        modelBuilder.Entity<LeaveBalance>(entity =>
        {
            entity.ToTable("LeaveBalances");
            entity.HasKey(x => new { x.EmployeeId, x.LeaveTypeId, x.Year });
            entity.HasOne(x => x.Employee)
                .WithMany(x => x.LeaveBalances)
                .HasForeignKey(x => x.EmployeeId);
            entity.HasOne(x => x.LeaveType)
                .WithMany(x => x.LeaveBalances)
                .HasForeignKey(x => x.LeaveTypeId);
            
            // Prevent negative balances
            entity.ToTable(table => table.HasCheckConstraint("CK_LeaveBalance_NonNegative", "RemainingDays >= 0"));
        });

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = (int)RoleType.Employee, Name = RoleNames.Employee },
            new Role { Id = (int)RoleType.Manager, Name = RoleNames.Manager },
            new Role { Id = (int)RoleType.Hr, Name = RoleNames.Hr },
            new Role { Id = (int)RoleType.Admin, Name = RoleNames.Admin });

        modelBuilder.Entity<LeaveType>().HasData(
            new LeaveType { Id = 1, Name = "Annual Leave", DefaultDays = 18 },
            new LeaveType { Id = 2, Name = "Sick Leave", DefaultDays = 10 },
            new LeaveType { Id = 3, Name = "Casual Leave", DefaultDays = 7 });
    }
}
