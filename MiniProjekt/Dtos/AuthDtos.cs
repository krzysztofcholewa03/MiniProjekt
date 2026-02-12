using System.ComponentModel.DataAnnotations;

namespace MiniProjekt.Dtos;

public record RegisterDto([Required, EmailAddress] string Email, [Required] string Password);
public record LoginDto([Required, EmailAddress] string Email, [Required] string Password);
public record AuthResponseDto(string Token);
