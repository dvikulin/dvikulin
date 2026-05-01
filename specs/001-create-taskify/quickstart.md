# Quickstart: Create Taskify

**Date**: 2026-05-01  
**Phase**: 1 - Setup and Development Guide  

## Prerequisites

- .NET 8 SDK
- PostgreSQL 15+
- Git
- Visual Studio 2022 or VS Code with C# extensions

## Project Setup

1. **Clone and navigate**:
   ```bash
   git clone <repository-url>
   cd Taskify
   git checkout 001-create-taskify
   ```

2. **Install .NET Aspire workload**:
   ```bash
   dotnet workload install aspire
   ```

3. **Setup PostgreSQL**:
   ```bash
   # Create database
   createdb taskify_dev

   # Or use Docker
   docker run --name postgres-taskify -e POSTGRES_DB=taskify_dev -e POSTGRES_PASSWORD=password -p 5432:5432 -d postgres:15
   ```

4. **Configure connection string**:
   Edit `Taskify.Api/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "TaskifyDb": "Host=localhost;Database=taskify_dev;Username=postgres;Password=password"
     }
   }
   ```

## Running the Application

1. **Start with .NET Aspire**:
   ```bash
   cd Taskify.AppHost
   dotnet run
   ```

2. **Access the application**:
   - Aspire Dashboard: http://localhost:15888
   - Taskify Web: http://localhost:PORT (shown in dashboard)
   - API: http://localhost:PORT (shown in dashboard)

3. **Initial setup**:
   - Select a user from the list (no password required)
   - View the projects page
   - Click on a project to see the Kanban board

## Development Workflow

### Adding a New Feature

1. **Write tests first** (TDD):
   ```bash
   cd tests/Taskify.Api.UnitTests
   dotnet test
   ```

2. **Implement API**:
   - Add model to `Taskify.Api/Models/`
   - Add service to `Taskify.Api/Services/`
   - Add controller to `Taskify.Api/Controllers/`

3. **Update database**:
   ```bash
   cd Taskify.Api
   dotnet ef migrations add AddNewFeature
   dotnet ef database update
   ```

4. **Implement UI**:
   - Add component to `Taskify.Web/Components/`
   - Update page in `Taskify.Web/Pages/`

### Running Tests

```bash
# Unit tests
dotnet test tests/Taskify.Api.UnitTests/

# Integration tests
dotnet test tests/Taskify.Web.IntegrationTests/

# Contract tests
dotnet test tests/Taskify.Api.ContractTests/
```

### Debugging

- Use Aspire dashboard for service logs
- Attach debugger to individual services
- Check PostgreSQL logs for database issues

## API Documentation

- Swagger UI: http://localhost:API-PORT/swagger
- API contracts in `specs/001-create-taskify/contracts/`

## Common Issues

### Database Connection
- Ensure PostgreSQL is running
- Check connection string in appsettings
- Run migrations: `dotnet ef database update`

### Real-time Updates Not Working
- Check SignalR connection in browser dev tools
- Verify WebSocket support
- Check firewall settings

### Drag-and-Drop Not Working
- Ensure modern browser (Chrome 88+, Firefox 85+)
- Check JavaScript console for errors
- Verify Blazor component rendering

## Architecture Overview

```
┌─────────────────┐    ┌─────────────────┐
│   Taskify.Web   │    │   Taskify.Api   │
│  (Blazor Server)│◄──►│  (REST API)     │
│                 │    │                 │
│ - User Selection│    │ - Projects API  │
│ - Projects List │    │ - Tasks API     │
│ - Kanban Board  │    │ - Notifications │
└─────────────────┘    └─────────────────┘
         │                       │
         └───────────────────────┘
                 ▲
                 │
        ┌─────────────────┐
        │  PostgreSQL     │
        │  Database       │
        └─────────────────┘
```

## Next Steps

- Run the checklist: Review `specs/001-create-taskify/checklist.md`
- Generate tasks: Run `/speckit.tasks` to create implementation tasks
- Start development: Begin with user selection feature