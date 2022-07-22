using Atlas_Web.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Helpers
{
    public static class UserHelpers
    {
        public static List<UserPreference> GetPreferences(
            IMemoryCache cache,
            Atlas_WebContext _context,
            string username
        )
        {
            username ??= "default";
            return cache.GetOrCreate<List<UserPreference>>(
                "Preferences-" + username,
                cacheEntry =>
                {
                    cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
                    return _context.UserPreferences
                        .Where(x => x.User.Username == username)
                        .ToList();
                }
            );
        }

        public static Boolean CheckHrxPermissions(
            Atlas_WebContext _context,
            int ReportObjectId,
            string username
        )
        {
            username ??= "default";

            var report = _context.ReportObjects
                .Where(x => x.ReportObjectId == ReportObjectId)
                .FirstOrDefault();

            //Epic - Crystal Report = 3
            // Reporting Workbench Report = 17

            if (report != null && report.ReportObjectTypeId != 3 && report.ReportObjectTypeId != 17)
            {
                return true;
            }

            // check hrx

            var hrx =
                (
                    from g in _context.ReportGroupsMemberships
                    where
                        g.ReportId == ReportObjectId
                        && (
                            from ug in _context.UserGroupsMemberships
                            where ug.User.Username == username
                            select ug.GroupId
                        ).Contains(g.GroupId)
                    select g.GroupId
                ).ToList().Count;
            if (hrx >= 1)
            {
                return true;
            }

            var hrg =
                (
                    from g in _context.ReportGroupsMemberships
                    join h in _context.ReportObjectHierarchies.Where(
                        x => x.ChildReportObjectId == ReportObjectId
                    )
                        on g.ReportId equals h.ParentReportObjectId
                    where
                        (
                            from ug in _context.UserGroupsMemberships
                            where ug.User.Username == username
                            select ug.GroupId
                        ).Contains(g.GroupId)
                    select g.GroupId
                ).ToList().Count;
            if (hrg >= 1)
            {
                return true;
            }

            return false;
        }
    }
}
