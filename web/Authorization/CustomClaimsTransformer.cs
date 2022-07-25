using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Atlas_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas_Web.Authorization;

public class CustomClaimsTransformer : IClaimsTransformation
{
    private readonly Atlas_WebContext _context;

    public CustomClaimsTransformer(Atlas_WebContext context)
    {
        _context = context;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity == null || string.IsNullOrEmpty(principal.Identity.Name))
            return principal;

        var userData = await GetUserData(principal.Identity.Name);

        var clone = principal.Clone();

        var claims = new List<Claim>()
        {
            new Claim("Fullname", userData.FullnameCalc),
            new Claim(ClaimTypes.Email, userData.Email),
            new Claim("Firstname", userData.FirstnameCalc),
            new Claim("UserId", userData.UserId.ToString(), ClaimValueTypes.Integer32),
        };

        // add user roles
        foreach (var role in userData.UserRoleLinks)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.UserRoles.Name));
        }
        // if they are an admin
        var isAdmin = userData.UserRoleLinks.Any(x => x.UserRoles.Name == "Administrator");
        // add active role
        var adminDisabled = userData.UserPreferences.FirstOrDefault(
            x => x.ItemType == "AdminDisabled"
        );

        // 1. All ways add the security points for the role "user"
        // 2. If the user is part of "Administrator", add admin roles unless they have have admin disabled.

        // add users permission points as roles
        foreach (
            var role in userData.UserRoleLinks
                .Where(x => x.UserRoles.Name != "User" && x.UserRoles.Name != "Administrator")
                .Select(x => x.UserRoles)
                .SelectMany(x => x.RolePermissionLinks)
                .Select(x => x.RolePermissions)
                .Union(
                    _context.RolePermissionLinks
                        .Where(x => x.Role.Name == "User")
                        .Select(x => x.RolePermissions)
                )
                .Union(_context.RolePermissions.Where(x => isAdmin && adminDisabled == null))
                .Distinct()
        )
        {
            claims.Add(new Claim("Permission", role.Name));
        }

        // add user groups
        foreach (var group in userData.UserGroupsMemberships)
        {
            claims.Add(new Claim("Group", group.GroupId.ToString()));
        }

        if (adminDisabled != null)
        {
            claims.Add(new Claim("AdminEnabled", "N", ClaimValueTypes.Integer32));
        }
        else
        {
            claims.Add(new Claim("AdminEnabled", "Y"));
        }

        ClaimsIdentity identity = new ClaimsIdentity(claims);

        clone.AddIdentity(identity);
        return clone;
    }

    public async Task<User> GetUserData(string username)
    {
        return await _context.Users
            .Include(x => x.UserRoleLinks)
            .ThenInclude(x => x.UserRoles)
            .ThenInclude(x => x.RolePermissionLinks)
            .ThenInclude(x => x.RolePermissions)
            .Include(x => x.UserPreferences)
            .Include(x => x.UserGroupsMemberships)
            .SingleAsync(x => x.Username == username);
    }
}
