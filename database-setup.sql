IF DB_ID(N'LeaveManagementSystemDb') IS NULL
BEGIN
    CREATE DATABASE LeaveManagementSystemDb;
END
GO

USE LeaveManagementSystemDb;
GO

IF OBJECT_ID(N'dbo.LeaveRequests', N'U') IS NOT NULL DROP TABLE dbo.LeaveRequests;
IF OBJECT_ID(N'dbo.LeaveBalances', N'U') IS NOT NULL DROP TABLE dbo.LeaveBalances;
IF OBJECT_ID(N'dbo.Employees', N'U') IS NOT NULL DROP TABLE dbo.Employees;
IF OBJECT_ID(N'dbo.LeaveTypes', N'U') IS NOT NULL DROP TABLE dbo.LeaveTypes;
IF OBJECT_ID(N'dbo.Roles', N'U') IS NOT NULL DROP TABLE dbo.Roles;
GO

CREATE TABLE dbo.Roles
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Roles PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL
);

CREATE UNIQUE INDEX IX_Roles_Name ON dbo.Roles(Name);

CREATE TABLE dbo.LeaveTypes
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_LeaveTypes PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    DefaultDays INT NOT NULL
);

CREATE UNIQUE INDEX IX_LeaveTypes_Name ON dbo.LeaveTypes(Name);

CREATE TABLE dbo.Employees
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Employees PRIMARY KEY,
    Email NVARCHAR(200) NOT NULL,
    Password NVARCHAR(500) NOT NULL,
    RoleId INT NOT NULL,
    ManagerId INT NULL,
    FullName NVARCHAR(150) NOT NULL,
    CONSTRAINT FK_Employees_Roles_RoleId FOREIGN KEY (RoleId) REFERENCES dbo.Roles(Id),
    CONSTRAINT FK_Employees_Employees_ManagerId FOREIGN KEY (ManagerId) REFERENCES dbo.Employees(Id)
);

CREATE UNIQUE INDEX IX_Employees_Email ON dbo.Employees(Email);
CREATE INDEX IX_Employees_RoleId ON dbo.Employees(RoleId);
CREATE INDEX IX_Employees_ManagerId ON dbo.Employees(ManagerId);

CREATE TABLE dbo.LeaveBalances
(
    EmployeeId INT NOT NULL,
    LeaveTypeId INT NOT NULL,
    Year INT NOT NULL,
    RemainingDays INT NOT NULL,
    CONSTRAINT PK_LeaveBalances PRIMARY KEY (EmployeeId, LeaveTypeId, Year),
    CONSTRAINT FK_LeaveBalances_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES dbo.Employees(Id),
    CONSTRAINT FK_LeaveBalances_LeaveTypes_LeaveTypeId FOREIGN KEY (LeaveTypeId) REFERENCES dbo.LeaveTypes(Id),
    CONSTRAINT CK_LeaveBalance_NonNegative CHECK (RemainingDays >= 0)
);

CREATE INDEX IX_LeaveBalances_LeaveTypeId ON dbo.LeaveBalances(LeaveTypeId);

CREATE TABLE dbo.LeaveRequests
(
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_LeaveRequests PRIMARY KEY,
    EmployeeId INT NOT NULL,
    LeaveTypeId INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Status NVARCHAR(30) NOT NULL,
    Reason NVARCHAR(1000) NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    ReviewedAtUtc DATETIME2 NULL,
    ReviewedByEmployeeId INT NULL,
    ManagerComment NVARCHAR(500) NULL,
    CONSTRAINT FK_LeaveRequests_Employees_EmployeeId FOREIGN KEY (EmployeeId) REFERENCES dbo.Employees(Id),
    CONSTRAINT FK_LeaveRequests_LeaveTypes_LeaveTypeId FOREIGN KEY (LeaveTypeId) REFERENCES dbo.LeaveTypes(Id),
    CONSTRAINT FK_LeaveRequests_Employees_ReviewedByEmployeeId FOREIGN KEY (ReviewedByEmployeeId) REFERENCES dbo.Employees(Id),
    CONSTRAINT CK_LeaveRequest_ValidDates CHECK (EndDate >= StartDate),
    CONSTRAINT CK_LeaveRequest_ValidStatus CHECK (Status IN ('Pending', 'Approved', 'Rejected'))
);

CREATE INDEX IX_LeaveRequests_EmployeeId ON dbo.LeaveRequests(EmployeeId);
CREATE INDEX IX_LeaveRequests_LeaveTypeId ON dbo.LeaveRequests(LeaveTypeId);
CREATE INDEX IX_LeaveRequests_ReviewedByEmployeeId ON dbo.LeaveRequests(ReviewedByEmployeeId);
GO

SET IDENTITY_INSERT dbo.Roles ON;
INSERT INTO dbo.Roles (Id, Name)
VALUES
    (1, N'Employee'),
    (2, N'Manager'),
    (3, N'HR'),
    (4, N'Admin');
SET IDENTITY_INSERT dbo.Roles OFF;

SET IDENTITY_INSERT dbo.LeaveTypes ON;
INSERT INTO dbo.LeaveTypes (Id, Name, DefaultDays)
VALUES
    (1, N'Annual Leave', 18),
    (2, N'Sick Leave', 10),
    (3, N'Casual Leave', 7);
SET IDENTITY_INSERT dbo.LeaveTypes OFF;

SET IDENTITY_INSERT dbo.Employees ON;
INSERT INTO dbo.Employees (Id, Email, Password, RoleId, ManagerId, FullName)
VALUES
    (1, N'admin@leaveapp.com', N'AQAAAAIAAYagAAAAEDKjqvJv6C9slKFj9gY8oq3kFvh9MSlLjaFfMjleRbXBXYu1I9TJHC7gMjC4tyvPKA==', 4, NULL, N'System Admin'),
    (2, N'hr@leaveapp.com', N'AQAAAAIAAYagAAAAECKsw/NFU8jhwtFXQ3MJpwvGUQKHG9rsudJcY5+nAwEkqd9CKSLaad9B1+PnJm1VNg==', 3, NULL, N'HR Executive'),
    (3, N'manager@leaveapp.com', N'AQAAAAIAAYagAAAAEDS7rLyZwVrsMrEMZnGilhYod0xXCA3nJZpZwKdE0ygJBxySHiE+uMNMpc+BS7mbBQ==', 2, NULL, N'Team Manager'),
    (4, N'employee@leaveapp.com', N'AQAAAAIAAYagAAAAEH85PDsdDLatZPFdUR4SPjw1A5IeTvnIMFNGSEeyrCFPs5+1Fq87GL7htVHBLJB77w==', 1, 3, N'Demo Employee');
SET IDENTITY_INSERT dbo.Employees OFF;

INSERT INTO dbo.LeaveBalances (EmployeeId, LeaveTypeId, Year, RemainingDays)
VALUES
    (1, 1, YEAR(GETDATE()), 18),
    (1, 2, YEAR(GETDATE()), 10),
    (1, 3, YEAR(GETDATE()), 7),
    (2, 1, YEAR(GETDATE()), 18),
    (2, 2, YEAR(GETDATE()), 10),
    (2, 3, YEAR(GETDATE()), 7),
    (3, 1, YEAR(GETDATE()), 18),
    (3, 2, YEAR(GETDATE()), 10),
    (3, 3, YEAR(GETDATE()), 7),
    (4, 1, YEAR(GETDATE()), 18),
    (4, 2, YEAR(GETDATE()), 9),
    (4, 3, YEAR(GETDATE()), 7);

SET IDENTITY_INSERT dbo.LeaveRequests ON;
INSERT INTO dbo.LeaveRequests
    (Id, EmployeeId, LeaveTypeId, StartDate, EndDate, Status, Reason, CreatedAtUtc, ReviewedAtUtc, ReviewedByEmployeeId, ManagerComment)
VALUES
    (1, 4, 1, CAST(DATEADD(DAY, 7, GETDATE()) AS DATE), CAST(DATEADD(DAY, 9, GETDATE()) AS DATE), N'Pending', N'Family function', SYSUTCDATETIME(), NULL, NULL, NULL),
    (2, 4, 2, CAST(DATEADD(DAY, 14, GETDATE()) AS DATE), CAST(DATEADD(DAY, 14, GETDATE()) AS DATE), N'Approved', N'Medical appointment', SYSUTCDATETIME(), SYSUTCDATETIME(), 3, N'Approved');
SET IDENTITY_INSERT dbo.LeaveRequests OFF;
GO
