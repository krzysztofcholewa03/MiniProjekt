using System.Security.Claims;

namespace MiniProjekt.Helpers;

public static class UserHelper
{
    public static string GetUserId(ClaimsPrincipal user)
        => user.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? throw new Exception("Brak user id w tokenie.");
}
