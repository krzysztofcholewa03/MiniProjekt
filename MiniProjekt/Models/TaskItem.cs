using System.ComponentModel.DataAnnotations;

namespace MiniProjekt.Models;

public class TaskItem
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = "";

    [MaxLength(2000)]
    public string? Description { get; set; }

    public bool IsDone { get; set; } = false;

    // Project
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    // Assigned user (optional)
    public string? AssignedUserId { get; set; }
    public ApplicationUser? AssignedUser { get; set; }
}
