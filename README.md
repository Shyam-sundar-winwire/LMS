# Leave Management System

A full-stack leave management application built with .NET 8 Web API, React + Vite, Material UI, and SQL Server. The solution includes JWT authentication, role-based authorization, leave application and approval workflows, auto-created leave balances, dashboard reporting, and seeded demo users.

## Tech Stack

- Backend: .NET 8 Web API with Clean Architecture
- Frontend: React + Vite + Material UI
- Database: SQL Server / LocalDB
- Authentication: JWT bearer token

## Solution Structure

```text
.
|-- backend/
|   `-- src/
|       |-- LeaveManagementSystem.API/
|       |-- LeaveManagementSystem.Application/
|       |-- LeaveManagementSystem.Domain/
|       `-- LeaveManagementSystem.Infrastructure/
|-- frontend/
|   `-- src/
|       |-- components/
|       |-- context/
|       |-- pages/
|       |-- services/
|       `-- utils/
`-- LeaveManagementSystem.slnx
```

## Backend Highlights

- Controller -> Service -> Repository flow
- DTO-based API responses and request contracts
- EF Core with SQL Server
- JWT login and role-based authorization via `[Authorize(Roles = ...)]`
- Global exception handling middleware
- Swagger with bearer token support
- Seed data for roles, users, leave types, balances, and one pending leave request
- Automatic leave balance creation when missing

## Frontend Highlights

- Apple-inspired polished UI with soft glassmorphism panels
- Role-based protected routing
- Persistent login using local storage
- Dashboard summaries by role
- Apply leave form
- My leaves tracking page
- Manager / HR / Admin approvals page
- HR / Admin all leave requests page
- Axios API client with auth interceptor

## Seeded Accounts

Use these seeded users to sign in after the API starts:

| Role | Email | Password |
|------|-------|----------|
| Admin | `admin@leaveapp.com` | `Admin@123` |
| HR | `hr@leaveapp.com` | `HR@123` |
| Manager | `manager@leaveapp.com` | `Manager@123` |
| Employee | `employee@leaveapp.com` | `Employee@123` |

## Database Setup

The API defaults to SQL Server LocalDB:

```json
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=LeaveManagementSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

If you want to use another SQL Server instance, update:

- `backend/src/LeaveManagementSystem.API/appsettings.json`

The application calls `EnsureCreated()` during startup and then seeds initial data when the database is empty.

## Run Locally

This repo includes a root `global.json` that pins the SDK to `.NET 8.0.419` so the backend runs with the expected toolchain even if newer SDKs are installed on your machine.

### 1. Start the backend

```powershell
cd F:\LMS
dotnet --version

cd backend\src\LeaveManagementSystem.API
dotnet run
```

Default development URLs from `launchSettings.json`:

- `https://localhost:7057`
- `http://localhost:5163`

Swagger UI:

- `https://localhost:7057/swagger`

### 2. Start the frontend

```powershell
cd frontend
npm install
npm run dev
```

Vite runs on:

- `http://localhost:5173`

If you change the backend URL, create `frontend/.env` and set:

```env
VITE_API_BASE_URL=https://localhost:7057/api
```

The frontend also falls back across the default local API URLs below during development if the first target is unavailable:

- `http://localhost:5163/api`
- `https://localhost:7057/api`

## API Surface

### Auth

- `POST /api/auth/login`

### Dashboard

- `GET /api/dashboard/summary`

### Leave Requests

- `GET /api/leaverequests/types`
- `GET /api/leaverequests/balances`
- `GET /api/leaverequests/mine`
- `GET /api/leaverequests/approvals`
- `GET /api/leaverequests/all`
- `POST /api/leaverequests`
- `PUT /api/leaverequests/{id}/review`

## Roles

- Employee: apply leave, view personal requests and balances
- Manager: employee actions plus team approvals
- HR: view all leaves, approve requests, see organization stats
- Admin: system-wide overview and full leave visibility

## Validation Completed

The following checks were completed successfully in this workspace:

- `dotnet build backend/src/LeaveManagementSystem.API/LeaveManagementSystem.API.csproj`
- `npm run build`

## Notes

- LocalDB (`MSSQLLocalDB`) is available on this machine and is the default target.
- Frontend dependencies were installed successfully and a `package-lock.json` was generated.
- `npm install` reported 2 moderate audit findings from the dependency tree. The application still builds successfully as-is.
