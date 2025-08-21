using System.Security.Claims;

namespace GeradorDeTestes.WebApp.Extensions;

public static class UserExtensions
{
    public static Guid? PegarIdUsuario(this ClaimsPrincipal user)
    {
        if (user is null)
            return null;

        string? id = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(id))
            return null;

        return Guid.Parse(id);
    }
}
