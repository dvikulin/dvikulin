---

description: "Task list template for feature implementation"
---

# Tasks: Create Taskify

**Input**: Design documents from `/specs/001-create-taskify/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/, quickstart.md

**Tests**: Tests are MANDATORY per TDD principle - write tests first, ensure they fail before implementation.

**Library-First**: Each feature must start with standalone library implementation.

**Documentation**: All code must be fully documented per constitution - include docstrings, API docs, etc.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **.NET Aspire**: `Taskify.AppHost/`, `Taskify.Api/`, `Taskify.Web/`, `Taskify.ServiceDefaults/`
- **Tests**: `tests/Taskify.Api.UnitTests/`, `tests/Taskify.Web.IntegrationTests/`, `tests/Taskify.Api.ContractTests/`
- Adjust paths based on plan.md structure

## Dependencies

User stories must be completed in priority order:
- US1 (Select User) → US2 (Create Projects) → US3 (Add Team Members) → US4 (Assign Tasks) → US5 (Comment on Tasks) → US6 (Move Tasks in Kanban)

Within each story, parallel tasks are marked [P].

## Implementation Strategy

- **MVP Scope**: Complete US1 (User Selection) for initial working application
- **Incremental Delivery**: Each user story delivers independently testable value
- **API-First**: Implement backend APIs before frontend UI
- **TDD**: Write failing tests before implementation
- **Parallel Execution**: Run independent tasks simultaneously within stories

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [X] T001 Create .NET Aspire solution structure per plan.md
- [X] T002 [P] Setup PostgreSQL database and connection strings
- [ ] T003 [P] Configure EF Core with migrations
- [X] T004 [P] Initialize xUnit test projects
- [X] T005 [P] Setup OpenAPI/Swagger documentation

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T006 Create User entity and seed data in Taskify.Api/Models/User.cs
- [X] T007 [P] Create Project entity in Taskify.Api/Models/Project.cs
- [X] T008 [P] Create Task entity in Taskify.Api/Models/Task.cs
- [X] T009 [P] Create Comment entity in Taskify.Api/Models/Comment.cs
- [X] T010 Create TaskifyDbContext in Taskify.Api/Data/TaskifyDbContext.cs
- [ ] T011 [P] Implement repository pattern base classes
- [X] T012 [P] Setup dependency injection configuration
- [X] T013 [P] Configure SignalR for real-time notifications
- [ ] T014 Create initial database migration

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Select User (Priority: P1) 🎯 MVP

**Goal**: Enable user selection from predefined list without authentication

**Independent Test**: Launch app, select user, verify main view loads

### Tests for User Story 1

- [X] T015 [P] [US1] Unit test for user selection logic in tests/Taskify.Api.UnitTests/
- [ ] T016 [P] [US1] Integration test for user data loading in tests/Taskify.Api.ContractTests/

### Implementation for User Story 1

- [X] T017 [US1] Create UsersController in Taskify.Api/Controllers/UsersController.cs
- [X] T018 [US1] Implement GET /api/users endpoint
- [X] T019 [US1] Create UserSelection.razor component in Taskify.Web/Pages/UserSelection.razor
- [X] T020 [US1] Add user selection UI with predefined users list
- [X] T021 [US1] Implement user selection navigation to projects view
- [X] T022 [US1] Add user session management (in-memory for now)

**Checkpoint**: US1 complete - basic app launch and user selection working

---

## Phase 4: User Story 2 - Create Projects (Priority: P2)

**Goal**: Allow creating and viewing projects

**Independent Test**: Create project, verify it appears in list

### Tests for User Story 2

- [ ] T023 [P] [US2] Unit tests for project CRUD operations
- [ ] T024 [P] [US2] Contract tests for projects API endpoints

### Implementation for User Story 2

- [X] T025 [US2] Create ProjectsController in Taskify.Api/Controllers/ProjectsController.cs
- [X] T026 [US2] Implement GET/POST /api/projects endpoints
- [X] T027 [US2] Create Projects.razor page in Taskify.Web/Pages/Projects.razor
- [X] T028 [US2] Add project creation form with validation
- [X] T029 [US2] Display sample projects on load
- [X] T030 [US2] Implement project selection navigation to Kanban

---

## Phase 5: User Story 3 - Add Team Members (Priority: P3)

**Goal**: Enable adding predefined users as team members to projects

**Independent Test**: Add user to project, verify team member list updates

### Tests for User Story 3

- [ ] T031 [P] [US3] Unit tests for team member management
- [ ] T032 [P] [US3] Integration tests for project membership

### Implementation for User Story 3

- [X] T033 [US3] Add team member endpoints to ProjectsController
- [X] T034 [US3] Implement POST/DELETE /api/projects/{id}/members
- [X] T035 [US3] Update Projects.razor to show team members
- [X] T036 [US3] Add team member management UI
- [X] T037 [US3] Validate only predefined users can be added

---

## Phase 6: User Story 4 - Assign Tasks (Priority: P4)

**Goal**: Allow creating tasks and assigning them to team members

**Independent Test**: Create task, assign to user, verify assignment

### Tests for User Story 4

- [ ] T038 [P] [US4] Unit tests for task assignment logic
- [ ] T039 [P] [US4] Contract tests for task creation and updates

### Implementation for User Story 4

- [X] T040 [US4] Create TasksController in Taskify.Api/Controllers/TasksController.cs
- [X] T041 [US4] Implement POST /api/tasks and PUT /api/tasks/{id}
- [X] T042 [US4] Create TaskCard.razor component in Taskify.Web/Components/TaskCard.razor
- [X] T043 [US4] Add task creation form in Kanban board
- [X] T044 [US4] Implement task assignment dropdown
- [X] T045 [US4] Validate assignee is project team member

---

## Phase 7: User Story 5 - Comment on Tasks (Priority: P5)

**Goal**: Enable commenting on tasks with edit/delete permissions

**Independent Test**: Add comment, edit own, verify others can't edit

### Tests for User Story 5

- [ ] T046 [P] [US5] Unit tests for comment CRUD with permissions
- [ ] T047 [P] [US5] Integration tests for comment real-time updates

### Implementation for User Story 5

- [X] T048 [US5] Add comment endpoints to TasksController
- [X] T049 [US5] Implement comment permissions (edit/delete own only)
- [X] T050 [US5] Update TaskCard.razor with comment section
- [X] T051 [US5] Add comment input and display
- [X] T052 [US5] Implement edit/delete comment UI with permissions

---

## Phase 8: User Story 6 - Move Tasks in Kanban (Priority: P6)

**Goal**: Enable drag-and-drop task movement with visual highlighting

**Independent Test**: Drag task between columns, verify status update and highlighting

### Tests for User Story 6

- [X] T053 [P] [US6] Unit tests for status transition logic
- [ ] T054 [P] [US6] UI integration tests for drag-and-drop

### Implementation for User Story 6

- [X] T055 [US6] Implement PUT /api/tasks/{id}/status endpoint
- [X] T056 [US6] Create KanbanBoard.razor page in Taskify.Web/Pages/KanbanBoard.razor
- [X] T057 [US6] Add drag-and-drop functionality with HTML5 API
- [X] T058 [US6] Implement visual highlighting for user's tasks
- [X] T059 [US6] Add real-time updates via SignalR
- [X] T060 [US6] Handle drag failures gracefully

---

## Phase 9: Integration & Polish

**Purpose**: Cross-cutting concerns and final integration

- [X] T061 [P] Add comprehensive input validation throughout APIs
- [X] T062 [P] Implement security headers and XSS prevention
- [X] T063 [P] Add error handling and user-friendly messages
- [X] T064 [P] Optimize database queries and add indexes
- [X] T065 [P] Add loading states and progress indicators
- [X] T066 [P] Implement responsive design for different screen sizes
- [X] T067 [P] Add accessibility features (ARIA labels, keyboard navigation)
- [X] T068 [P] Create comprehensive API documentation
- [X] T069 [P] Add application logging and monitoring
- [ ] T070 [P] Performance testing and optimization
- [ ] T071 [P] End-to-end testing across all user stories
- [ ] T072 [P] Documentation review and updates
