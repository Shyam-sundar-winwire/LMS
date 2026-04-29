#  Leave Management System

A full-stack enterprise-style application designed to streamline employee leave workflows — from request submission to approval and tracking.

Built using **.NET 8 Web API** and **React**, this project demonstrates real-world architecture, secure authentication, and role-based access control.

---

##  Key Highlights

-  Secure JWT-based authentication with role-based access control (RBAC)
-  Multi-role system: Employee, Manager, HR, Admin
-  Complete leave lifecycle (apply → approve/reject → track)
-  Dynamic dashboards based on user roles
-  Leave balance tracking by type and year
-  Clean layered backend architecture (API, Business, Domain, Infrastructure)
-  Unit tested backend using xUnit, Moq, and EF Core InMemory
-  Protected frontend routes with persistent login state

---

##  Tech Stack

### Backend

- .NET 8 Web API
- Entity Framework Core

### Frontend

- React (Vite)
- Material UI

### Database

- SQL Server / SQL Server LocalDB

### Authentication

- JWT (JSON Web Tokens)

### Testing

- xUnit
- Moq
- EF Core InMemory Provider

---

##  Project Structure

```text
.
├── backend/
│   └── src/
│       ├── LeaveManagementSystem.API/
│       ├── LeaveManagementSystem.Business/
│       ├── LeaveManagementSystem.Domain/
│       ├── LeaveManagementSystem.Infrastructure/
│       └── LeaveManagementSystem.Services.Tests/
├── frontend/
│   └── src/
│       ├── components/
│       ├── context/
│       ├── pages/
│       ├── services/
│       └── utils/
├── global.json
├── LMS.sln
└── LeaveManagementSystem.slnx
```

##  Features

- User authentication with JWT tokens
- Role-based authorization (Employee, Manager, HR, Admin)
- Submit leave requests
- Approve or reject requests
- Track leave balances
- View personal and organization-wide dashboards
- Pre-seeded demo data for quick testing
- Consistent API error handling
- Persistent frontend login sessions

##  Demo Accounts

Use these credentials after starting the backend:

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@leaveapp.com` | `Admin@123` |
| HR | `hr@leaveapp.com` | `HR@123` |
| Manager | `manager@leaveapp.com` | `Manager@123` |
| Employee | `employee@leaveapp.com` | `Employee@123` |

##  Database Configuration

By default, the application uses SQL Server LocalDB:

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=LeaveManagementSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

To use a different SQL Server instance, update:

```text
backend/src/LeaveManagementSystem.API/appsettings.json
```

The database is automatically created and seeded on application startup.

##  Run Locally

The repository includes a `global.json` file that pins the .NET SDK version to `8.0.419`.

###  Backend

```powershell
cd backend/src/LeaveManagementSystem.API
dotnet run
```

Runs on:

- `https://localhost:7057`
- `http://localhost:5163`

Swagger:

- `https://localhost:7057/swagger`

###  Frontend

```powershell
cd frontend
npm install
npm run dev
```

Runs on:

- `http://localhost:5173`

Optional `.env` configuration:

```env
VITE_API_BASE_URL=https://localhost:7057/api
```

## API Overview

###  Auth

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Login and receive JWT token |

###  Dashboard

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/dashboard/summary` | Get role-based dashboard metrics |

###  Leave Requests

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/leaverequests/types` | Get leave types |
| GET | `/api/leaverequests/balances` | Get leave balances |
| GET | `/api/leaverequests/mine` | Get user requests |
| GET | `/api/leaverequests/approvals` | Get pending approvals |
| GET | `/api/leaverequests/all` | Get all requests (HR/Admin) |
| POST | `/api/leaverequests` | Apply for leave |
| PUT | `/api/leaverequests/{id}/review` | Approve/Reject request |

##  Roles & Permissions

| Role | Capabilities |
|------|--------------|
| Employee | Apply leave, view personal requests, track balances |
| Manager | Employee access + approve team requests |
| HR | View all requests, manage approvals, view insights |
| Admin | Full system access |

##  Testing

Run backend tests:

```powershell
dotnet test backend/src/LeaveManagementSystem.Services.Tests/LeaveManagementSystem.Services.Tests.csproj
```

Build frontend:

```powershell
cd frontend
npm run build
```

##  Configuration

Important files:

- `backend/src/LeaveManagementSystem.API/appsettings.json`
- `backend/src/LeaveManagementSystem.API/appsettings.Development.json`
- `frontend/.env.example`
- `global.json`

 Do NOT commit:

- JWT secrets
- Production database credentials

##  Final Note

This project follows a real-world scalable architecture, with clear separation of concerns across backend layers and a modern frontend structure.

It is designed to reflect how enterprise applications are built and maintained.
