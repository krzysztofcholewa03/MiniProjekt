using System.ComponentModel.DataAnnotations;

namespace MiniProjekt.Models;

public class Project
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = "";

    [MaxLength(1000)]
    public string? Description { get; set; }

    // Owner
    [Required]
    public string OwnerId { get; set; } = "";
    public ApplicationUser Owner { get; set; } = null!;

    // Tasks
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
