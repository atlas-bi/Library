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

        if (userData == null)
        {
            // need a better handling if users don't exist.
#pragma warning disable S112
            throw new Exception("User " + principal.Identity.Name + " does not exist.");
#pragma warning restore S112

        }

        var clone = principal.Clone();

        var claims = new List<Claim>()
        {
            new Claim("Fullname", userData.FullnameCalc ?? principal.Identity.Name),
            new Claim(ClaimTypes.Email, userData.Email ?? ""),
            new Claim("Firstname", userData.FirstnameCalc ?? ""),
            new Claim("UserId", userData.UserId.ToString(), ClaimValueTypes.Integer32),
        };

        // add user roles + group roles
        foreach (
            var role in userData.UserRoleLinks
                .Select(x => x.UserRoles)
                .Union(
                    userData.UserGroupsMemberships
                        .Select(x => x.Group)
                        .SelectMany(x => x.GroupRoleLinks)
                        .Select(x => x.UserRoles)
                )
                .Select(role => role.Name)
        )
        {
            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }
        // if they are an admin
        var isAdmin =
            userData.UserRoleLinks.Any(x => x.UserRoles.Name == "Administrator")
            || userData.UserGroupsMemberships.Any(
                x => x.Group.GroupRoleLinks.Any(x => x.UserRoles.Name == "Administrator")
            );

        // add active role
        var adminDisabled = userData.UserPreferences.FirstOrDefault(
            x => x.ItemType == "AdminDisabled"
        );

        // 1. All ways add the security points for the role "user"
        // 2. If the user is part of "Administrator", add admin roles unless they have have admin disabled.

        // add users permission points as roles
        foreach (
            // permissions from the users roles
            var role in userData.UserRoleLinks
                .Where(x => x.UserRoles.Name != "User" && x.UserRoles.Name != "Administrator")
                .Select(x => x.UserRoles)
                .SelectMany(x => x.RolePermissionLinks)
                .Select(x => x.RolePermissions)
                // union in the base user permissions
                .Union(
                    _context.RolePermissionLinks
                        .Where(x => x.Role.Name == "User")
                        .Select(x => x.RolePermissions)
                )
                // if user is admin, add all admin permissions
                .Union(_context.RolePermissions.Where(x => isAdmin && adminDisabled == null))
                // if user is in a group, add the groups permissions (excluding user and admin)
                .Union(
                    userData.UserGroupsMemberships
                        .SelectMany(x => x.Group.GroupRoleLinks)
                        .Where(
                            x => x.UserRoles.Name != "Administrator" && x.UserRoles.Name != "User"
                        )
                        .Select(x => x.UserRoles)
                        .SelectMany(x => x.RolePermissionLinks)
                        .Select(x => x.RolePermissions)
                )
                .Distinct()
                .Select(role => role.Name)
        )
        {
            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim("Permission", role));
            }
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
        User me = await _context.Users
            .Include(x => x.UserRoleLinks)
            .ThenInclude(x => x.UserRoles)
            .ThenInclude(x => x.RolePermissionLinks)
            .ThenInclude(x => x.RolePermissions)
            .Include(x => x.UserPreferences)
            .Include(x => x.UserGroupsMemberships)
            .Include(x => x.UserGroupsMemberships)
            .ThenInclude(x => x.Group)
            .ThenInclude(x => x.GroupRoleLinks)
            .ThenInclude(x => x.UserRoles)
            .ThenInclude(x => x.RolePermissionLinks)
            .ThenInclude(x => x.RolePermissions)
            .SingleOrDefaultAsync(x => x.Username == username);

        if (me == null)
        {
            await _context.AddAsync(
                new User
                {
                    Username = username,
                    FullnameCalc = "Guest",
                    FirstnameCalc = "Guest"
                }
            );
            await _context.SaveChangesAsync();

            // if this is the first user in the db (new install), make them an admin.
            if (await _context.Users.CountAsync() == 1)
            {
                await _context.AddAsync(
                    new UserRoleLink
                    {
                        UserId = _context.Users.Where(x => x.Username == username).First().UserId,
                        UserRolesId =
                            _context.UserRoles
                                .Where(x => x.Name == "Administrator")
                                .First().UserRolesId
                    }
                );
                await _context.SaveChangesAsync();
            }

            me = await _context.Users
                .Include(x => x.UserRoleLinks)
                .ThenInclude(x => x.UserRoles)
                .ThenInclude(x => x.RolePermissionLinks)
                .ThenInclude(x => x.RolePermissions)
                .SingleAsync(x => x.Username == username);
        }

        return me;
    }
}
