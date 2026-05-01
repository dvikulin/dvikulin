using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taskify.Database.Models;

public class Comment
{
    public int Id { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public int AuthorId { get; set; }

    [Required]
    public int TaskId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey(nameof(AuthorId))]
    public User Author { get; set; } = null!;

    [ForeignKey(nameof(TaskId))]
    public Task Task { get; set; } = null!;
}