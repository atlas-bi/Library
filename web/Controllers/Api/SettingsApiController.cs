using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Settings;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Controllers.Api;

[ApiController]
[Route("api/settings")]
[Authorize(AuthenticationSchemes = "Bearer")]
public sealed class SettingsApiController : ControllerBase
{
    private readonly Atlas_WebContext _context;
    private readonly IMemoryCache _cache;
    private readonly IWebHostEnvironment _environment;

    public SettingsApiController(Atlas_WebContext context, IMemoryCache cache, IWebHostEnvironment environment)
    {
        _context = context;
        _cache = cache;
        _environment = environment;
    }

    [HttpGet("site-messages")]
    public async Task<ActionResult<IReadOnlyList<SiteMessageDto>>> GetSiteMessages(CancellationToken cancellationToken = default)
    {
        return Ok(await _context.GlobalSiteSettings.Where(x => x.Name == "msg")
            .Select(x => new SiteMessageDto { Id = x.Id, Value = x.Value, Description = x.Description })
            .ToListAsync(cancellationToken));
    }

    [HttpPost("site-messages")]
    public async Task<ActionResult<SiteMessageDto>> AddSiteMessage(SiteMessageRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!User.HasPermission("Manage Global Site Settings")) return Forbid();
        if (request == null || string.IsNullOrWhiteSpace(request.Value)) return BadRequest(new { error = "Value is required." });
        var message = new GlobalSiteSetting { Name = "msg", Value = request.Value, Description = request.Description };
        _context.GlobalSiteSettings.Add(message);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new SiteMessageDto { Id = message.Id, Value = message.Value, Description = message.Description });
    }

    [HttpDelete("site-messages/{id:int}")]
    public async Task<IActionResult> DeleteSiteMessage(int id, CancellationToken cancellationToken = default)
    {
        if (!User.HasPermission("Manage Global Site Settings")) return Forbid();
        var message = await _context.GlobalSiteSettings.SingleOrDefaultAsync(x => x.Id == id && x.Name == "msg", cancellationToken);
        if (message == null) return NotFound();
        _context.Remove(message);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("etl")]
    public async Task<ActionResult<SettingValueDto>> GetEtl(CancellationToken cancellationToken = default) =>
        Ok(new SettingValueDto { Value = await GetSettingValue("report_tag_etl", cancellationToken) });

    [HttpPut("etl")]
    public async Task<IActionResult> UpdateEtl(SettingValueDto request, CancellationToken cancellationToken = default)
    {
        if (!User.HasPermission("Manage Global Site Settings")) return Forbid();
        await SetSettingValue("report_tag_etl", request?.Value, cancellationToken);
        return NoContent();
    }

    [HttpGet("etl/default")]
    public async Task<IActionResult> GetDefaultEtl(CancellationToken cancellationToken = default)
    {
        var path = Path.Combine(_environment.WebRootPath, "defaults", "report_tags_etl.sql");
        return Content(await System.IO.File.ReadAllTextAsync(path, cancellationToken));
    }

    [HttpGet("theme")]
    public async Task<ActionResult<SettingValueDto>> GetTheme(CancellationToken cancellationToken = default) =>
        Ok(new SettingValueDto { Value = await GetSettingValue("global_css", cancellationToken) });

    [HttpPut("theme")]
    public async Task<IActionResult> UpdateTheme(SettingValueDto request, CancellationToken cancellationToken = default)
    {
        if (!User.HasPermission("Manage Global Site Settings")) return Forbid();
        await SetSettingValue("global_css", request?.Value, cancellationToken);
        _cache.Set("global_css", request?.Value);
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<SearchSettingsDto>> GetSearch(CancellationToken cancellationToken = default)
    {
        var values = await _context.GlobalSiteSettings.Where(x => x.Name.EndsWith("_search_visibility"))
            .ToDictionaryAsync(x => x.Name[..^18], x => x.Value, cancellationToken);
        var reportTypes = await _context.ReportObjectTypes.OrderBy(x => x.Name)
            .Select(x => new SearchReportTypeDto { Id = x.ReportObjectTypeId, Name = x.Name, ShortName = x.ShortName, Visible = x.Visible == "Y" })
            .ToListAsync(cancellationToken);
        return Ok(new SearchSettingsDto { Visibility = values, ReportTypes = reportTypes });
    }

    [HttpPut("search/{type}/visibility")]
    public async Task<IActionResult> UpdateSearchVisibility(string type, SearchVisibilityRequestDto request, [FromQuery] int? reportTypeId = null, CancellationToken cancellationToken = default)
    {
        if (!User.HasPermission("Manage Global Site Settings")) return Forbid();
        if (type == "reports")
        {
            var reportType = await _context.ReportObjectTypes.SingleOrDefaultAsync(x => x.ReportObjectTypeId == reportTypeId, cancellationToken);
            if (reportType == null) return NotFound();
            reportType.Visible = request.Visible ? "Y" : "N";
        }
        else
        {
            if (string.IsNullOrWhiteSpace(type)) return BadRequest();
            var setting = await _context.GlobalSiteSettings.SingleOrDefaultAsync(x => x.Name == type + "_search_visibility", cancellationToken);
            if (setting == null) _context.GlobalSiteSettings.Add(new GlobalSiteSetting { Name = type + "_search_visibility", Value = request.Visible ? "Y" : "N" });
            else setting.Value = request.Visible ? "Y" : "N";
        }
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPut("search/report-types/{id:int}/text")]
    public async Task<IActionResult> UpdateSearchText(int id, SearchTextRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!User.HasPermission("Manage Global Site Settings")) return Forbid();
        var reportType = await _context.ReportObjectTypes.FindAsync([id], cancellationToken);
        if (reportType == null) return NotFound();
        reportType.ShortName = request?.Text;
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("permissions")]
    public async Task<ActionResult<IReadOnlyList<PermissionDto>>> GetPermissions(CancellationToken cancellationToken = default)
    {
        return Ok(await _context.RolePermissions.OrderBy(x => x.Name)
            .Select(x => new PermissionDto { Id = x.RolePermissionsId, Name = x.Name })
            .ToListAsync(cancellationToken));
    }

    [HttpGet("roles")]
    public async Task<ActionResult<IReadOnlyList<RoleDto>>> GetRoles(CancellationToken cancellationToken = default)
    {
        return Ok(await _context.UserRoles.Include(x => x.RolePermissionLinks).ThenInclude(x => x.RolePermissions)
            .Select(x => new RoleDto { Id = x.UserRolesId, Name = x.Name, Permissions = x.RolePermissionLinks.OrderBy(y => y.RolePermissions.Name).Select(y => new PermissionDto { Id = y.RolePermissionsId, Name = y.RolePermissions.Name }).ToList() })
            .ToListAsync(cancellationToken));
    }

    [HttpPost("roles")]
    public async Task<ActionResult<RoleDto>> CreateRole(RoleRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!User.HasPermission("Edit Role Permissions")) return Forbid();
        if (request == null || string.IsNullOrWhiteSpace(request.Name) || request.Name is "Administrator" or "Director") return BadRequest(new { error = "A valid role name is required." });
        var role = new UserRole { Name = request.Name };
        _context.UserRoles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new RoleDto { Id = role.UserRolesId, Name = role.Name, Permissions = [] });
    }

    [HttpDelete("roles/{id:int}")]
    public async Task<IActionResult> DeleteRole(int id, CancellationToken cancellationToken = default)
    {
        if (!User.HasPermission("Edit Role Permissions")) return Forbid();
        var role = await _context.UserRoles.FindAsync([id], cancellationToken);
        if (role == null) return NotFound();
        if (id is 1 or 5 || role.Name == "Director") return BadRequest(new { error = "This role cannot be deleted." });
        _context.RemoveRange(_context.UserRoleLinks.Where(x => x.UserRolesId == id));
        _context.RemoveRange(_context.RolePermissionLinks.Where(x => x.RoleId == id));
        _context.Remove(role);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPut("roles/{roleId:int}/permissions/{permissionId:int}")]
    public async Task<IActionResult> UpdateRolePermission(int roleId, int permissionId, RolePermissionRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!User.HasPermission("Edit Role Permissions")) return Forbid();
        var link = await _context.RolePermissionLinks.SingleOrDefaultAsync(x => x.RoleId == roleId && x.RolePermissionsId == permissionId, cancellationToken);
        if (request?.Enabled == true && link == null) _context.RolePermissionLinks.Add(new RolePermissionLink { RoleId = roleId, RolePermissionsId = permissionId });
        if (request?.Enabled != true && link != null) _context.RolePermissionLinks.Remove(link);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("user-roles")]
    public async Task<ActionResult<IReadOnlyList<UserRoleAssignmentDto>>> GetUserRoles(CancellationToken cancellationToken = default)
    {
        return Ok(await _context.Users.Where(x => x.UserRoleLinks.Any(y => y.UserRoles.Name.ToLower() != "user"))
            .OrderBy(x => x.Username).Select(x => new UserRoleAssignmentDto { UserId = x.UserId, Name = x.FullnameCalc, Roles = x.UserRoleLinks.Select(y => new PermissionDto { Id = y.UserRolesId, Name = y.UserRoles.Name }).ToList() }).ToListAsync(cancellationToken));
    }

    [HttpPost("user-roles/{userId:int}")]
    public async Task<IActionResult> AddUserRole(int userId, RoleAssignmentRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!User.HasPermission("Edit User Permissions")) return Forbid();
        if (request == null || !await _context.Users.AnyAsync(x => x.UserId == userId, cancellationToken) || !await _context.UserRoles.AnyAsync(x => x.UserRolesId == request.RoleId, cancellationToken)) return BadRequest(new { error = "User and role are required." });
        if (!await _context.UserRoleLinks.AnyAsync(x => x.UserId == userId && x.UserRolesId == request.RoleId, cancellationToken))
        {
            _context.UserRoleLinks.Add(new UserRoleLink { UserId = userId, UserRolesId = request.RoleId });
            await _context.SaveChangesAsync(cancellationToken);
        }
        return NoContent();
    }

    [HttpDelete("user-roles/{userId:int}/{roleId:int}")]
    public async Task<IActionResult> RemoveUserRole(int userId, int roleId, CancellationToken cancellationToken = default)
    {
        if (!User.HasPermission("Edit User Permissions")) return Forbid();
        _context.UserRoleLinks.RemoveRange(_context.UserRoleLinks.Where(x => x.UserId == userId && x.UserRolesId == roleId));
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("group-roles")]
    public async Task<ActionResult<IReadOnlyList<GroupRoleAssignmentDto>>> GetGroupRoles(CancellationToken cancellationToken = default)
    {
        return Ok(await _context.UserGroups.Where(x => x.GroupRoleLinks.Any(y => y.UserRoles.Name.ToLower() != "user"))
            .OrderBy(x => x.GroupName).Select(x => new GroupRoleAssignmentDto { GroupId = x.GroupId, Name = x.GroupName, Roles = x.GroupRoleLinks.Select(y => new PermissionDto { Id = y.UserRolesId, Name = y.UserRoles.Name }).ToList() }).ToListAsync(cancellationToken));
    }

    [HttpPost("group-roles/{groupId:int}")]
    public async Task<IActionResult> AddGroupRole(int groupId, RoleAssignmentRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!User.HasPermission("Edit Group Permissions")) return Forbid();
        if (request == null || !await _context.UserGroups.AnyAsync(x => x.GroupId == groupId, cancellationToken) || !await _context.UserRoles.AnyAsync(x => x.UserRolesId == request.RoleId, cancellationToken)) return BadRequest(new { error = "Group and role are required." });
        if (!await _context.GroupRoleLinks.AnyAsync(x => x.GroupId == groupId && x.UserRolesId == request.RoleId, cancellationToken))
        {
            _context.GroupRoleLinks.Add(new GroupRoleLink { GroupId = groupId, UserRolesId = request.RoleId });
            await _context.SaveChangesAsync(cancellationToken);
        }
        return NoContent();
    }

    [HttpDelete("group-roles/{groupId:int}/{roleId:int}")]
    public async Task<IActionResult> RemoveGroupRole(int groupId, int roleId, CancellationToken cancellationToken = default)
    {
        if (!User.HasPermission("Edit Group Permissions")) return Forbid();
        _context.GroupRoleLinks.RemoveRange(_context.GroupRoleLinks.Where(x => x.GroupId == groupId && x.UserRolesId == roleId));
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpGet("tags/{type}")]
    public async Task<ActionResult<IReadOnlyList<ParameterDto>>> GetTags(string type, CancellationToken cancellationToken = default)
    {
        return Ok(await ParameterQuery(type).ToListAsync(cancellationToken));
    }

    [HttpPost("tags/{type}")]
    public async Task<ActionResult<ParameterDto>> CreateTag(string type, ParameterRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!User.HasPermission("Create Parameters")) return Forbid();
        if (request == null || string.IsNullOrWhiteSpace(request.Name)) return BadRequest(new { error = "Name is required." });
        var entity = CreateParameter(type, request);
        if (entity == null) return NotFound();
        _context.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(new ParameterDto { Id = ParameterId(entity), Name = request.Name, Description = request.Description });
    }

    [HttpDelete("tags/{type}/{id:int}")]
    public async Task<IActionResult> DeleteTag(string type, int id, CancellationToken cancellationToken = default)
    {
        if (!User.HasPermission("Delete Parameters")) return Forbid();
        var entity = await FindParameter(type, id, cancellationToken);
        if (entity == null) return NotFound();
        DetachParameter(type, id);
        _context.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private async Task<string> GetSettingValue(string name, CancellationToken token) => await _context.GlobalSiteSettings.Where(x => x.Name == name).Select(x => x.Value).FirstOrDefaultAsync(token);

    private async Task SetSettingValue(string name, string value, CancellationToken token)
    {
        var setting = await _context.GlobalSiteSettings.SingleOrDefaultAsync(x => x.Name == name, token);
        if (setting == null) _context.GlobalSiteSettings.Add(new GlobalSiteSetting { Name = name, Value = value }); else setting.Value = value;
        await _context.SaveChangesAsync(token);
    }

    private IQueryable<ParameterDto> ParameterQuery(string type) => type.ToLowerInvariant() switch
    {
        "organizational-values" => _context.OrganizationalValues.Select(x => new ParameterDto { Id = x.Id, Name = x.Name, Used = x.ReportObjectDocs.Count }),
        "estimated-run-frequencies" => _context.EstimatedRunFrequencies.Select(x => new ParameterDto { Id = x.Id, Name = x.Name, Used = x.ReportObjectDocs.Count }),
        "maintenance-schedules" => _context.MaintenanceSchedules.Select(x => new ParameterDto { Id = x.Id, Name = x.Name, Used = x.ReportObjectDocs.Count }),
        "fragilities" => _context.Fragilities.Select(x => new ParameterDto { Id = x.Id, Name = x.Name, Used = x.ReportObjectDocs.Count }),
        "fragility-tags" => _context.FragilityTags.Select(x => new ParameterDto { Id = x.Id, Name = x.Name, Used = x.ReportObjectDocFragilityTags.Count }),
        "tags" => _context.Tags.Select(x => new ParameterDto { Id = x.TagId, Name = x.Name, Description = x.Description, Used = x.ReportTagLinks.Count }),
        "maintenance-log-statuses" => _context.MaintenanceLogStatuses.Select(x => new ParameterDto { Id = x.Id, Name = x.Name, Used = x.MaintenanceLogs.Count }),
        "financial-impacts" => _context.FinancialImpacts.Select(x => new ParameterDto { Id = x.Id, Name = x.Name, Used = x.Initiatives.Count + x.Collections.Count }),
        "strategic-importances" => _context.StrategicImportances.Select(x => new ParameterDto { Id = x.Id, Name = x.Name, Used = x.Initiatives.Count + x.Collections.Count }),
        _ => throw new KeyNotFoundException(type),
    };

    private object CreateParameter(string type, ParameterRequestDto request) => type.ToLowerInvariant() switch
    {
        "organizational-values" => new OrganizationalValue { Name = request.Name },
        "estimated-run-frequencies" => new EstimatedRunFrequency { Name = request.Name },
        "maintenance-schedules" => new MaintenanceSchedule { Name = request.Name },
        "fragilities" => new Fragility { Name = request.Name },
        "fragility-tags" => new FragilityTag { Name = request.Name },
        "tags" => new Tag { Name = request.Name, Description = request.Description },
        "maintenance-log-statuses" => new MaintenanceLogStatus { Name = request.Name },
        "financial-impacts" => new FinancialImpact { Name = request.Name },
        "strategic-importances" => new StrategicImportance { Name = request.Name },
        _ => null,
    };

    private int ParameterId(object entity) => entity switch
    {
        Tag x => x.TagId, _ => (int)entity.GetType().GetProperty("Id")!.GetValue(entity),
    };

    private async Task<object> FindParameter(string type, int id, CancellationToken token) => type.ToLowerInvariant() switch
    {
        "organizational-values" => await _context.OrganizationalValues.FindAsync([id], token),
        "estimated-run-frequencies" => await _context.EstimatedRunFrequencies.FindAsync([id], token),
        "maintenance-schedules" => await _context.MaintenanceSchedules.FindAsync([id], token),
        "fragilities" => await _context.Fragilities.FindAsync([id], token),
        "fragility-tags" => await _context.FragilityTags.FindAsync([id], token),
        "tags" => await _context.Tags.FindAsync([id], token),
        "maintenance-log-statuses" => await _context.MaintenanceLogStatuses.FindAsync([id], token),
        "financial-impacts" => await _context.FinancialImpacts.FindAsync([id], token),
        "strategic-importances" => await _context.StrategicImportances.FindAsync([id], token),
        _ => null,
    };

    private void DetachParameter(string type, int id)
    {
        switch (type.ToLowerInvariant())
        {
            case "organizational-values": _context.ReportObjectDocs.Where(x => x.OrganizationalValueId == id).ToList().ForEach(x => x.OrganizationalValueId = null); break;
            case "estimated-run-frequencies": _context.ReportObjectDocs.Where(x => x.EstimatedRunFrequencyId == id).ToList().ForEach(x => x.EstimatedRunFrequencyId = null); break;
            case "maintenance-schedules": _context.ReportObjectDocs.Where(x => x.MaintenanceScheduleId == id).ToList().ForEach(x => x.MaintenanceScheduleId = null); break;
            case "fragilities": _context.ReportObjectDocs.Where(x => x.FragilityId == id).ToList().ForEach(x => x.FragilityId = null); break;
            case "fragility-tags": _context.RemoveRange(_context.ReportObjectDocFragilityTags.Where(x => x.FragilityTagId == id)); break;
            case "tags": _context.RemoveRange(_context.ReportTagLinks.Where(x => x.TagId == id)); break;
            case "maintenance-log-statuses": _context.RemoveRange(_context.MaintenanceLogs.Where(x => x.MaintenanceLogStatusId == id)); break;
            case "financial-impacts": _context.Collections.Where(x => x.FinancialImpact == id).ToList().ForEach(x => x.FinancialImpact = null); _context.Initiatives.Where(x => x.FinancialImpact == id).ToList().ForEach(x => x.FinancialImpact = null); break;
            case "strategic-importances": _context.Collections.Where(x => x.StrategicImportance == id).ToList().ForEach(x => x.StrategicImportance = null); _context.Initiatives.Where(x => x.StrategicImportance == id).ToList().ForEach(x => x.StrategicImportance = null); break;
        }
    }
}
