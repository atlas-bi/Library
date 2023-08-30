using Atlas_Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace Atlas_Web.Authorization
{
    public class ReportRunAuthorizationHandler
        : AuthorizationHandler<PermissionRequirement, ReportObject>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement,
            ReportObject report
        )
        {
            // Only catch - Crystal Report (3) & Reporting Workbench (17)
            if (
                report.ReportObjectType.Name != "Epic-Crystal Report"
                && report.ReportObjectType.Name != "Reporting Workbench Report"
            )
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var groups = context.User.Claims
                .Where(x => x.Type == "Group")
                .Select(x => x.Value)
                .ToList();

            // check hrx
            if (report.ReportGroupsMemberships.Any(x => groups.Contains(x.GroupId.ToString())))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // check hrg
            if (
                report.ReportObjectHierarchyChildReportObjects.Any(
                    x =>
                        x.ParentReportObject.ReportGroupsMemberships.Any(
                            y => groups.Contains(y.GroupId.ToString())
                        )
                )
            )
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }

    public class PermissionRequirement : IAuthorizationRequirement { }
}
