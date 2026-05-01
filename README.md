# Taskify

Taskify is a small team productivity app for managing projects and tasks on a Kanban board. It is built as a .NET Aspire application with a Blazor Server frontend, an ASP.NET Core API, Entity Framework Core, PostgreSQL, and SignalR for live project-board updates.

## What It Does

- Shows five predefined teammates on launch: one product manager and four engineers
- Lets a user enter the app by selecting a teammate, with no password or login flow
- Displays three seeded sample projects
- Lets users create new projects
- Lets users add predefined teammates to project teams
- Lets users create tasks and assign them to project members
- Shows project tasks in Kanban columns: To Do, In Progress, In Review, and Done
- Supports drag-and-drop task movement between columns
- Highlights tasks assigned to the currently selected user
- Lets users add comments to tasks
- Lets users edit or delete only their own comments

## Solution Structure

```text
Taskify.AppHost/          .NET Aspire orchestration
Taskify.Api/              REST API and SignalR notifications
Taskify.Database/         EF Core DbContext and domain models
Taskify.Web/              Blazor Server UI
Taskify.ServiceDefaults/  Shared Aspire service defaults
Taskify.Tests/            xUnit tests
specs/001-create-taskify/ Spec Kit feature documents and tasks
```

## Local Requirements

- .NET 10 SDK
- Docker Desktop, for the Aspire-managed PostgreSQL container
- A modern browser

## Run Locally

From the repository root:

```bash
dotnet restore Taskify.slnx
dotnet run --project Taskify.AppHost/Taskify.AppHost.csproj --launch-profile https
```

Open the Aspire dashboard URL printed in the terminal, or go directly to the web app:

```text
http://localhost:5002
```

The API usually runs at:

```text
http://localhost:5001
```

## Test

```bash
dotnet build Taskify.slnx --no-restore
dotnet test Taskify.slnx --no-build
```

## Current Status

The app has a working MVP flow: select user, view projects, create projects, manage team members, open a Kanban board, create tasks, move tasks, and manage comments. Remaining follow-up work is tracked in `specs/001-create-taskify/tasks.md`, including deeper integration tests, UI end-to-end tests, EF migrations, and final documentation polish.
