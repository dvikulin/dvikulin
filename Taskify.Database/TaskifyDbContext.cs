using Microsoft.EntityFrameworkCore;
using Taskify.Database.Models;
using TaskModel = Taskify.Database.Models.Task;

namespace Taskify.Database;

public class TaskifyDbContext : DbContext
{
    public TaskifyDbContext(DbContextOptions<TaskifyDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<TaskModel> Tasks { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var seededAt = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<ProjectMember>()
            .HasKey(pm => new { pm.ProjectId, pm.UserId });

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Name)
            .IsUnique();

        modelBuilder.Entity<Project>()
            .HasIndex(p => p.Name)
            .IsUnique();

        modelBuilder.Entity<Project>()
            .HasOne(p => p.Owner)
            .WithMany(u => u.OwnedProjects)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProjectMember>()
            .HasOne(pm => pm.Project)
            .WithMany(p => p.Members)
            .HasForeignKey(pm => pm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjectMember>()
            .HasOne(pm => pm.User)
            .WithMany(u => u.ProjectMemberships)
            .HasForeignKey(pm => pm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TaskModel>()
            .Property(t => t.Status)
            .HasConversion<string>();

        modelBuilder.Entity<TaskModel>()
            .Property(t => t.Priority)
            .HasConversion<string>();

        modelBuilder.Entity<TaskModel>()
            .HasIndex(t => new { t.ProjectId, t.Status });

        modelBuilder.Entity<TaskModel>()
            .HasOne(t => t.Assignee)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Author)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Task)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Alice Johnson", Email = "alice@taskify.local", Role = "Product Manager", CreatedAt = seededAt, UpdatedAt = seededAt },
            new User { Id = 2, Name = "Bob Smith", Email = "bob@taskify.local", Role = "Engineer", CreatedAt = seededAt, UpdatedAt = seededAt },
            new User { Id = 3, Name = "Carol Williams", Email = "carol@taskify.local", Role = "Engineer", CreatedAt = seededAt, UpdatedAt = seededAt },
            new User { Id = 4, Name = "David Brown", Email = "david@taskify.local", Role = "Engineer", CreatedAt = seededAt, UpdatedAt = seededAt },
            new User { Id = 5, Name = "Eve Davis", Email = "eve@taskify.local", Role = "Engineer", CreatedAt = seededAt, UpdatedAt = seededAt });

        modelBuilder.Entity<Project>().HasData(
            new Project { Id = 1, Name = "Website Redesign", Description = "Refresh the company website with a clearer information architecture and modern visuals.", OwnerId = 1, CreatedAt = seededAt, UpdatedAt = seededAt },
            new Project { Id = 2, Name = "Mobile App Development", Description = "Build the first mobile companion experience for Taskify users.", OwnerId = 1, CreatedAt = seededAt, UpdatedAt = seededAt },
            new Project { Id = 3, Name = "API Integration", Description = "Connect Taskify to internal systems and expose integration-ready endpoints.", OwnerId = 1, CreatedAt = seededAt, UpdatedAt = seededAt });

        modelBuilder.Entity<ProjectMember>().HasData(
            new ProjectMember { ProjectId = 1, UserId = 1, Role = ProjectRole.Owner, JoinedAt = seededAt },
            new ProjectMember { ProjectId = 1, UserId = 2, Role = ProjectRole.Member, JoinedAt = seededAt },
            new ProjectMember { ProjectId = 1, UserId = 3, Role = ProjectRole.Member, JoinedAt = seededAt },
            new ProjectMember { ProjectId = 2, UserId = 1, Role = ProjectRole.Owner, JoinedAt = seededAt },
            new ProjectMember { ProjectId = 2, UserId = 2, Role = ProjectRole.Member, JoinedAt = seededAt },
            new ProjectMember { ProjectId = 2, UserId = 4, Role = ProjectRole.Member, JoinedAt = seededAt },
            new ProjectMember { ProjectId = 3, UserId = 1, Role = ProjectRole.Owner, JoinedAt = seededAt },
            new ProjectMember { ProjectId = 3, UserId = 3, Role = ProjectRole.Member, JoinedAt = seededAt },
            new ProjectMember { ProjectId = 3, UserId = 5, Role = ProjectRole.Member, JoinedAt = seededAt });

        modelBuilder.Entity<TaskModel>().HasData(
            new TaskModel { Id = 1, ProjectId = 1, Title = "Design homepage mockup", Description = "Create a responsive homepage mockup for stakeholder review.", Status = Models.TaskStatus.Todo, Priority = TaskPriority.High, AssigneeId = 1, CreatedAt = seededAt, UpdatedAt = seededAt },
            new TaskModel { Id = 2, ProjectId = 1, Title = "Build navigation shell", Description = "Implement the main responsive navigation and project entry points.", Status = Models.TaskStatus.InProgress, Priority = TaskPriority.Medium, AssigneeId = 2, CreatedAt = seededAt, UpdatedAt = seededAt },
            new TaskModel { Id = 3, ProjectId = 2, Title = "Implement user onboarding", Description = "Create the first-pass onboarding screen flow for mobile users.", Status = Models.TaskStatus.InReview, Priority = TaskPriority.High, AssigneeId = 4, CreatedAt = seededAt, UpdatedAt = seededAt },
            new TaskModel { Id = 4, ProjectId = 3, Title = "Create REST endpoints", Description = "Expose project and task endpoints for integration consumers.", Status = Models.TaskStatus.Done, Priority = TaskPriority.Urgent, AssigneeId = 3, CreatedAt = seededAt, UpdatedAt = seededAt });

        modelBuilder.Entity<Comment>().HasData(
            new Comment { Id = 1, TaskId = 1, AuthorId = 2, Content = "I added a first pass at the content blocks for the mockup.", CreatedAt = seededAt, UpdatedAt = seededAt },
            new Comment { Id = 2, TaskId = 3, AuthorId = 1, Content = "Please make sure the onboarding copy stays short enough for small screens.", CreatedAt = seededAt, UpdatedAt = seededAt });
    }
}
