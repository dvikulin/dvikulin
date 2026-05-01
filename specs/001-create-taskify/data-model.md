# Data Model: Create Taskify

**Date**: 2026-05-01  
**Phase**: 1 - Design and Modeling  

## Database Schema

### Users Table
```sql
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    role VARCHAR(50) NOT NULL CHECK (role IN ('Product Manager', 'Engineer')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

**Notes**: Predefined users loaded on startup. Roles determine permissions.

### Projects Table
```sql
CREATE TABLE projects (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

**Notes**: Projects contain tasks and have team members.

### ProjectMembers Table (Many-to-Many)
```sql
CREATE TABLE project_members (
    project_id INTEGER REFERENCES projects(id) ON DELETE CASCADE,
    user_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    joined_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (project_id, user_id)
);
```

**Notes**: Links users to projects as team members.

### Tasks Table
```sql
CREATE TABLE tasks (
    id SERIAL PRIMARY KEY,
    project_id INTEGER REFERENCES projects(id) ON DELETE CASCADE,
    title VARCHAR(300) NOT NULL,
    description TEXT,
    status VARCHAR(50) NOT NULL DEFAULT 'To Do' CHECK (status IN ('To Do', 'In Progress', 'In Review', 'Done')),
    assignee_id INTEGER REFERENCES users(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

**Notes**: Tasks belong to projects, have status for Kanban columns, optional assignee.

### Comments Table
```sql
CREATE TABLE comments (
    id SERIAL PRIMARY KEY,
    task_id INTEGER REFERENCES tasks(id) ON DELETE CASCADE,
    user_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    content TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

**Notes**: Comments on tasks, with author tracking for edit/delete permissions.

## Entity Relationships

```
Users (1) -- (*) ProjectMembers (*) -- (1) Projects
Projects (1) -- (*) Tasks
Tasks (1) -- (*) Comments
Users (1) -- (*) Tasks (assignee)
Users (1) -- (*) Comments
```

## Data Constraints

- User names must be unique
- Project names must be unique
- Task titles cannot be empty
- Comments cannot be empty
- Only project members can be task assignees
- Status transitions are unrestricted (any to any)

## Initial Data

### Users
1. Alice Johnson (Product Manager)
2. Bob Smith (Engineer)
3. Carol Williams (Engineer)
4. David Brown (Engineer)
5. Eve Davis (Engineer)

### Sample Projects
1. Website Redesign
2. Mobile App Development
3. API Integration

### Sample Tasks
- Website Redesign: "Design homepage mockup" (To Do, assigned to Alice)
- Mobile App Development: "Implement user authentication" (In Progress, assigned to Bob)
- API Integration: "Create REST endpoints" (Done, assigned to Carol)

## EF Core Configuration

```csharp
public class TaskifyDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure relationships and constraints
        modelBuilder.Entity<ProjectMember>()
            .HasKey(pm => new { pm.ProjectId, pm.UserId });

        modelBuilder.Entity<Task>()
            .Property(t => t.Status)
            .HasConversion<string>();

        // Seed initial data
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Alice Johnson", Role = "Product Manager" },
            // ... other users
        );
    }
}
```

## Migration Strategy

- Use EF Core migrations for schema changes
- Initial migration includes schema and seed data
- Future migrations for feature additions

## Performance Considerations

- Indexes on foreign keys (project_id, user_id, task_id)
- Composite index on (project_id, status) for Kanban queries
- Pagination for large task lists
- Connection pooling for PostgreSQL