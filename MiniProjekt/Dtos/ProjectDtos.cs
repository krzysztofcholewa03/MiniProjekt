using System.ComponentModel.DataAnnotations;

namespace MiniProjekt.Dtos;

public record ProjectCreateDto([Required] string Name, string? Description);
public record ProjectUpdateDto([Required] string Name, string? Description);
