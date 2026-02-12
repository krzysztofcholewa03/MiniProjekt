using System.ComponentModel.DataAnnotations;

namespace MiniProjekt.Dtos;

public record TaskCreateDto([Required] string Title, string? Description, int ProjectId, string? AssignedUserId);
public record TaskUpdateDto([Required] string Title, string? Description, bool IsDone, int ProjectId, string? AssignedUserId);
