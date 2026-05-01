using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taskify.Database.Models;

public class ProjectMember
{
    [Required]
    public int ProjectId { get; set; }

    [Required]
    public int UserId { get; set; }

    public ProjectRole Role { get; set; } = ProjectRole.Member;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(ProjectId))]
    public Project Project { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}