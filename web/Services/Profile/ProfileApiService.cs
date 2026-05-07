using System.Text.RegularExpressions;
using Atlas_Web.Contracts.Api.Profile;
using Atlas_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas_Web.Services;

public interface IProfileApiService
{
    Task<ProfileChartResponseDto> GetChartAsync(
        int id,
        string type,
        double startAt,
        double endAt,
        List<string> server,
        List<string> database,
        List<string> masterFile,
        List<string> visible,
        List<string> certification,
        List<string> availability,
        List<int> reportType,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<ProfileBarItemDto>> GetUsersAsync(
        int id,
        string type,
        double startAt,
        double endAt,
        List<string> server,
        List<string> database,
        List<string> masterFile,
        List<string> visible,
        List<string> certification,
        List<string> availability,
        List<int> reportType,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<ProfileBarItemDto>> GetReportsAsync(
        int id,
        string type,
        double startAt,
        double endAt,
        List<string> server,
        List<string> database,
        List<string> masterFile,
        List<string> visible,
        List<string> certification,
        List<string> availability,
        List<int> reportType,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<ProfileBarItemDto>> GetFailsAsync(
        int id,
        string type,
        double startAt,
        double endAt,
        List<string> server,
        List<string> database,
        List<string> masterFile,
        List<string> visible,
        List<string> certification,
        List<string> availability,
        List<int> reportType,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<ProfileRunListItemDto>> GetRunListAsync(
        int id,
        string type,
        List<int> reportType,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<ProfileStarUserDto>> GetStarsAsync(
        int id,
        string type,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<ProfileSubscriptionDto>> GetSubscriptionsAsync(
        int id,
        string type,
        CancellationToken cancellationToken
    );
}

public sealed class ProfileApiService : IProfileApiService
{
    private sealed class ProfileRunRow
    {
        public int? RunUserId { get; init; }
        public User RunUser { get; init; }
        public DateTime RunStartTime { get; init; }
        public DateTime RunStartTime_Hour { get; init; }
        public DateTime RunStartTime_Day { get; init; }
        public DateTime RunStartTime_Month { get; init; }
        public int RunDurationSeconds { get; init; }
        public string RunStatus { get; init; }
        public int Runs { get; init; }
        public int ReportObjectId { get; init; }
        public string Name { get; init; }
        public string DisplayTitle { get; init; }
    }

    private readonly Atlas_WebContext _context;
    private readonly IConfiguration _config;

    public ProfileApiService(Atlas_WebContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<ProfileChartResponseDto> GetChartAsync(
        int id,
        string type,
        double startAt,
        double endAt,
        List<string> server,
        List<string> database,
        List<string> masterFile,
        List<string> visible,
        List<string> certification,
        List<string> availability,
        List<int> reportType,
        CancellationToken cancellationToken
    )
    {
        var (subquery, subqueryGroup, dateFormat) = await BuildSubqueriesAsync(
            id,
            type,
            startAt,
            endAt,
            server,
            database,
            masterFile,
            visible,
            certification,
            availability,
            reportType,
            cancellationToken
        );

        var history = await (
            from grp in subqueryGroup
            orderby grp.Key
            select new ProfileRunHistoryPointDto
            {
                Date = grp.Key.ToString(dateFormat),
                Users = grp.Select(x => x.RunUserId).Distinct().Count(),
                Runs = grp.Sum(x => x.Runs),
                RunTime = Math.Round(grp.Average(x => (int)x.RunDurationSeconds), 1),
            }
        )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var totalRuns = history.Sum(x => x.Runs);
        var distinctUsers = await subquery.Select(x => x.RunUserId).Distinct().CountAsync(cancellationToken);
        var averageRunTime = totalRuns > 0
            ? Math.Round(await subquery.AverageAsync(x => (int)x.RunDurationSeconds, cancellationToken), 2)
            : 0;

        return new ProfileChartResponseDto
        {
            Runs = totalRuns,
            Users = distinctUsers,
            RunTime = averageRunTime,
            History = history,
        };
    }

    public async Task<IReadOnlyList<ProfileBarItemDto>> GetUsersAsync(
        int id,
        string type,
        double startAt,
        double endAt,
        List<string> server,
        List<string> database,
        List<string> masterFile,
        List<string> visible,
        List<string> certification,
        List<string> availability,
        List<int> reportType,
        CancellationToken cancellationToken
    )
    {
        var (subquery, _, _) = await BuildSubqueriesAsync(
            id,
            type,
            startAt,
            endAt,
            server,
            database,
            masterFile,
            visible,
            certification,
            availability,
            reportType,
            cancellationToken
        );

        var total = await subquery.SumAsync(x => x.Runs, cancellationToken);
        return await (
            from a in subquery
            group a by new { a.RunUserId, a.RunUser.FullnameCalc } into grp
            select new ProfileBarItemDto
            {
                Key = grp.Key.FullnameCalc,
                Count = grp.Sum(x => x.Runs),
                Percent = total == 0 ? 0 : (double)grp.Sum(x => x.Runs) / total,
                TitleOne = "Top Users",
                Date = grp.Max(x => x.RunStartTime).ToShortDateString(),
                DateTitle = "Last Run",
                TitleTwo = "Runs",
                Href = IsUserProfileEnabled() ? "/users?id=" + grp.Key.RunUserId : null,
            }
        )
            .OrderByDescending(x => x.Count)
            .Take(20)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProfileBarItemDto>> GetReportsAsync(
        int id,
        string type,
        double startAt,
        double endAt,
        List<string> server,
        List<string> database,
        List<string> masterFile,
        List<string> visible,
        List<string> certification,
        List<string> availability,
        List<int> reportType,
        CancellationToken cancellationToken
    )
    {
        var (subquery, _, _) = await BuildSubqueriesAsync(
            id,
            type,
            startAt,
            endAt,
            server,
            database,
            masterFile,
            visible,
            certification,
            availability,
            reportType,
            cancellationToken
        );

        var total = await subquery.SumAsync(x => x.Runs, cancellationToken);
        return await (
            from a in subquery
            group a by new
            {
                a.ReportObjectId,
                Name = string.IsNullOrEmpty(a.DisplayTitle) ? a.Name : a.DisplayTitle,
            } into grp
            select new ProfileBarItemDto
            {
                Key = grp.Key.Name,
                Count = grp.Sum(x => x.Runs),
                Percent = total == 0 ? 0 : (double)grp.Sum(x => x.Runs) / total,
                TitleOne = "Top Reports",
                Date = grp.Max(x => x.RunStartTime).ToShortDateString(),
                DateTitle = "Last Run",
                TitleTwo = "Runs",
                Href = "/reports?id=" + grp.Key.ReportObjectId,
            }
        )
            .OrderByDescending(x => x.Count)
            .Take(20)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProfileBarItemDto>> GetFailsAsync(
        int id,
        string type,
        double startAt,
        double endAt,
        List<string> server,
        List<string> database,
        List<string> masterFile,
        List<string> visible,
        List<string> certification,
        List<string> availability,
        List<int> reportType,
        CancellationToken cancellationToken
    )
    {
        var (subquery, _, _) = await BuildSubqueriesAsync(
            id,
            type,
            startAt,
            endAt,
            server,
            database,
            masterFile,
            visible,
            certification,
            availability,
            reportType,
            cancellationToken
        );

        var total = await subquery.SumAsync(x => x.Runs, cancellationToken);
        return await (
            from a in subquery
            where a.RunStatus != "Success"
            group a by a.RunStatus into grp
            select new ProfileBarItemDto
            {
                Key = Regex.Replace(
                    Regex.Replace(grp.Key, @"^rs", "", RegexOptions.Multiline),
                    @"(?<=[a-z])([A-Z])",
                    " $1"
                ),
                Count = grp.Sum(x => x.Runs),
                Percent = total == 0 ? 0 : (double)grp.Sum(x => x.Runs) / total,
                TitleOne = "Failed Runs",
                TitleTwo = "Fails",
            }
        )
            .OrderByDescending(x => x.Count)
            .Take(20)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProfileRunListItemDto>> GetRunListAsync(
        int id,
        string type,
        List<int> reportType,
        CancellationToken cancellationToken
    )
    {
        reportType ??= new List<int>();
        var runData = _context.ReportObjectRunDatas.AsQueryable();

        if (string.Equals(type, "report", StringComparison.OrdinalIgnoreCase))
        {
            return await (
                from b in runData
                from d in b.ReportObjectRunDataBridges
                where d.ReportObjectId == id
                group new { b, d } by new { b.RunUserId, b.RunUser.FullnameCalc } into grp
                orderby grp.Max(x => x.b.RunStartTime) descending
                select new ProfileRunListItemDto
                {
                    Name = grp.Key.FullnameCalc,
                    Url = IsUserProfileEnabled() ? "\\users?id=" + grp.Key.RunUserId : null,
                    Runs = grp.Sum(x => x.d.Runs),
                    LastRun = grp.Max(x => x.b.RunStartTime).ToShortDateString(),
                }
            )
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        if (string.Equals(type, "user", StringComparison.OrdinalIgnoreCase))
        {
            runData = runData.Where(x => x.RunUserId == id);
        }
        else if (string.Equals(type, "group", StringComparison.OrdinalIgnoreCase))
        {
            runData = runData.Where(x => x.RunUser.UserGroupsMemberships.Any(g => g.GroupId == id));
        }

        var reports = _context.ReportObjects.AsQueryable();
        if (reportType.Count > 0)
        {
            reports = reports.Where(x => reportType.Contains((int)x.ReportObjectTypeId));
        }

        return await (
            from r in reports
            join b in _context.ReportObjectRunDataBridges on r.ReportObjectId equals b.ReportObjectId
            join d in runData on b.RunId equals d.RunDataId
            where b.Inherited == 0
            group new { r, b, d } by new
            {
                r.ReportObjectId,
                Name = string.IsNullOrEmpty(r.DisplayTitle) ? r.Name : r.DisplayTitle,
                r.ReportObjectType.ShortName,
                ReportTypeName = r.ReportObjectType.Name,
            } into grp
            orderby grp.Max(x => x.d.RunStartTime) descending
            select new ProfileRunListItemDto
            {
                Name = grp.Key.Name,
                Type = string.IsNullOrEmpty(grp.Key.ShortName)
                    ? grp.Key.ReportTypeName
                    : grp.Key.ShortName,
                Url = "\\reports?id=" + grp.Key.ReportObjectId,
                Runs = grp.Sum(x => x.b.Runs),
                LastRun = grp.Max(x => x.d.RunStartTime).ToShortDateString(),
            }
        )
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProfileStarUserDto>> GetStarsAsync(
        int id,
        string type,
        CancellationToken cancellationToken
    )
    {
        return type switch
        {
            "report" when await _context.ReportObjects.AnyAsync(x => x.ReportObjectId == id, cancellationToken) =>
                await _context.Users.Where(x => x.StarredReports.Any(r => r.Reportid == id))
                    .Select(x => new ProfileStarUserDto
                    {
                        Id = x.UserId,
                        FullName = x.FullnameCalc,
                        Email = x.Email,
                    })
                    .AsNoTracking()
                    .ToListAsync(cancellationToken),
            "term" when await _context.Terms.AnyAsync(x => x.TermId == id, cancellationToken) =>
                await _context.Users.Where(x => x.StarredTerms.Any(r => r.Termid == id))
                    .Select(x => new ProfileStarUserDto
                    {
                        Id = x.UserId,
                        FullName = x.FullnameCalc,
                        Email = x.Email,
                    })
                    .AsNoTracking()
                    .ToListAsync(cancellationToken),
            "collection" when await _context.Collections.AnyAsync(x => x.CollectionId == id, cancellationToken) =>
                await _context.Users.Where(x => x.StarredCollections.Any(r => r.Collectionid == id))
                    .Select(x => new ProfileStarUserDto
                    {
                        Id = x.UserId,
                        FullName = x.FullnameCalc,
                        Email = x.Email,
                    })
                    .AsNoTracking()
                    .ToListAsync(cancellationToken),
            _ => throw new InvalidOperationException(
                "Wrong parameter value supplied. Type: " + type + " with Id: " + id
            ),
        };
    }

    public async Task<IReadOnlyList<ProfileSubscriptionDto>> GetSubscriptionsAsync(
        int id,
        string type,
        CancellationToken cancellationToken
    )
    {
        if (
            !string.Equals(type, "report", StringComparison.OrdinalIgnoreCase)
            || !await _context.ReportObjects.AnyAsync(x => x.ReportObjectId == id, cancellationToken)
        )
        {
            throw new InvalidOperationException(
                "Wrong parameter value supplied. Type: " + type + " with Id: " + id
            );
        }

        return await _context.ReportObjectSubscriptions.Where(r => r.ReportObjectId == id)
            .Include(x => x.User)
            .Select(x => new ProfileSubscriptionDto
            {
                Id = x.ReportObjectSubscriptionsId,
                UserId = x.UserId,
                UserName = x.User != null ? x.User.FullnameCalc : null,
                EmailList = x.EmailList,
                Description = x.Description,
                LastStatus = x.LastStatus,
                LastRunTime = x.LastRunTime,
                SubscriptionTo = x.SubscriptionTo,
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    private async Task<
        Tuple<
            IQueryable<ProfileRunRow>,
            IQueryable<IGrouping<DateTime, ProfileRunRow>>,
            string
        >
    > BuildSubqueriesAsync(
        int id,
        string type,
        double startAt,
        double endAt,
        List<string> server,
        List<string> database,
        List<string> masterFile,
        List<string> visible,
        List<string> certification,
        List<string> availability,
        List<int> reportType,
        CancellationToken cancellationToken
    )
    {
        server ??= new List<string>();
        database ??= new List<string>();
        masterFile ??= new List<string>();
        visible ??= new List<string>();
        certification ??= new List<string>();
        availability ??= new List<string>();
        reportType ??= new List<int>();

        var start = DateTime.Now.AddSeconds(startAt);
        var end = DateTime.Now.AddSeconds(endAt);

        var runData = _context.ReportObjectRunDatas.AsQueryable();
        var reports = _context.ReportObjects.AsQueryable();

        if (string.Equals(type, "report", StringComparison.OrdinalIgnoreCase))
        {
            if (id == -1)
            {
                reports = ApplyReportFilters(
                    reports,
                    server,
                    database,
                    masterFile,
                    visible,
                    certification,
                    availability,
                    reportType
                );
            }
            else if (await _context.ReportObjects.AnyAsync(x => x.ReportObjectId == id, cancellationToken))
            {
                reports = reports.Where(x => x.ReportObjectId == id);
            }
            else
            {
                throw InvalidType(type, id);
            }
        }
        else if (
            string.Equals(type, "term", StringComparison.OrdinalIgnoreCase)
            && await _context.Terms.AnyAsync(x => x.TermId == id, cancellationToken)
        )
        {
            reports = reports.Where(x =>
                _context.ReportObjectDocTerms.Where(t => t.TermId == id)
                    .Select(t => t.ReportObjectId)
                    .Contains(x.ReportObjectId)
            );
        }
        else if (
            string.Equals(type, "collection", StringComparison.OrdinalIgnoreCase)
            && await _context.Collections.AnyAsync(x => x.CollectionId == id, cancellationToken)
        )
        {
            reports = reports.Where(x =>
                _context.CollectionReports.Where(c => c.CollectionId == id)
                    .Select(c => c.ReportId)
                    .Contains(x.ReportObjectId)
            );
        }
        else if (
            string.Equals(type, "user", StringComparison.OrdinalIgnoreCase)
            && await _context.Users.AnyAsync(x => x.UserId == id, cancellationToken)
        )
        {
            runData = runData.Where(x => x.RunUserId == id);
            reports = ApplyReportFilters(
                reports,
                server,
                database,
                masterFile,
                visible,
                certification,
                availability,
                reportType
            );
        }
        else if (
            string.Equals(type, "group", StringComparison.OrdinalIgnoreCase)
            && await _context.UserGroups.AnyAsync(x => x.GroupId == id, cancellationToken)
        )
        {
            runData = runData.Where(x => x.RunUser.UserGroupsMemberships.Any(g => g.GroupId == id));
            reports = ApplyReportFilters(
                reports,
                server,
                database,
                masterFile,
                visible,
                certification,
                availability,
                reportType
            );
        }
        else
        {
            throw InvalidType(type, id);
        }

        var joined = from d in runData
            join b in _context.ReportObjectRunDataBridges on d.RunDataId equals b.RunId
            join r in reports on b.ReportObjectId equals r.ReportObjectId
            select new ProfileRunRow
            {
                RunUserId = d.RunUserId,
                RunUser = d.RunUser,
                RunStartTime = d.RunStartTime,
                RunStartTime_Hour = d.RunStartTime_Hour,
                RunStartTime_Day = d.RunStartTime_Day,
                RunStartTime_Month = d.RunStartTime_Month,
                RunDurationSeconds = d.RunDurationSeconds ?? 0,
                RunStatus = d.RunStatus,
                Runs = b.Runs,
                ReportObjectId = b.ReportObjectId,
                Name = r.Name,
                DisplayTitle = r.DisplayTitle,
            };

        string dateFormat;
        IQueryable<IGrouping<DateTime, ProfileRunRow>> grouped;

        if (endAt - startAt < 172800)
        {
            dateFormat = "h tt";
            grouped = joined.Where(x => x.RunStartTime_Hour >= start && x.RunStartTime_Hour <= end)
                .GroupBy(x => x.RunStartTime_Hour);
        }
        else if (endAt - startAt < 31536000)
        {
            dateFormat = endAt - startAt < 691200 ? "ddd M/d" : "MMM d";
            grouped = joined.Where(x => x.RunStartTime_Day >= start && x.RunStartTime_Day <= end)
                .GroupBy(x => x.RunStartTime_Day);
        }
        else
        {
            dateFormat = "MMM yy";
            grouped = joined.Where(x => x.RunStartTime_Month >= start && x.RunStartTime_Month <= end)
                .GroupBy(x => x.RunStartTime_Month);
        }

        var flattened = grouped.SelectMany(x => x);
        return Tuple.Create(flattened, grouped, dateFormat);
    }

    private IQueryable<ReportObject> ApplyReportFilters(
        IQueryable<ReportObject> reports,
        List<string> server,
        List<string> database,
        List<string> masterFile,
        List<string> visible,
        List<string> certification,
        List<string> availability,
        List<int> reportType
    )
    {
        if (server.Count > 0)
        {
            reports = reports.Where(x => server.Contains(x.SourceServer));
        }

        if (database.Count > 0)
        {
            reports = reports.Where(x => database.Contains(x.SourceDb));
        }

        if (masterFile.Count > 0)
        {
            reports = reports.Where(x =>
                masterFile.Contains(x.EpicMasterFile)
                || (masterFile.Contains("None") && string.IsNullOrEmpty(x.EpicMasterFile))
            );
        }

        if (visible.Count > 0)
        {
            reports = reports.Where(x =>
                visible.Contains(x.DefaultVisibilityYn)
                || (visible.Contains("Y") && string.IsNullOrEmpty(x.DefaultVisibilityYn))
            );
        }

        if (certification.Count > 0)
        {
            reports = reports.Where(x =>
                certification.Intersect(x.ReportTagLinks.Select(y => y.Tag.Name)).Any()
            );
        }

        if (availability.Count > 0)
        {
            reports = reports.Where(x =>
                availability.Contains(x.Availability)
                || (availability.Contains("Public") && string.IsNullOrEmpty(x.Availability))
            );
        }

        if (reportType.Count > 0)
        {
            reports = reports.Where(x => reportType.Contains((int)x.ReportObjectTypeId));
        }

        return reports;
    }

    private bool IsUserProfileEnabled()
    {
        return _config["features:enable_user_profile"] == null
            || _config["features:enable_user_profile"].ToLower() == "true";
    }

    private static InvalidOperationException InvalidType(string type, int id)
    {
        return new InvalidOperationException(
            "Wrong parameter value supplied. Type: " + type + " with Id: " + id
        );
    }
}
