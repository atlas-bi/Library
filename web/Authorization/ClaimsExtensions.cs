using System.Security.Claims;

namespace Atlas_Web.Authorization;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.Email);
    }

    public static int GetUserId(this ClaimsPrincipal principal)
    {
        return Int32.Parse(principal.FindFirstValue("UserId"));
    }

    public static string GetUserName(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("Fullname");
    }

    public static string HasAdminEnabled(this ClaimsPrincipal principal)
    {
        return string.IsNullOrEmpty(principal.FindFirstValue("AdminEnabled"))
            ? "Y"
            : principal.FindFirstValue("AdminEnabled");
    }

    public static bool HasPermission(this ClaimsPrincipal principal, string claim)
    {
        return principal.HasClaim("Permission", claim);
    }

    public static bool IsCurrentUser(this ClaimsPrincipal principal, int id)
    {
        var currentUserId = GetUserId(principal);

        return id == currentUserId;
    }
}
