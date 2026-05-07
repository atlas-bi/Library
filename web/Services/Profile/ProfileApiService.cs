using System.Text.RegularExpressions;
using Atlas_Web.Contracts.Api.Profile;
using Atlas_Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas_Web.Services;

public interface IProfileApiService
{
    Task<ProfileChartResponseDto> GetChartAsync(
        ProfileQueryRequestDto request,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<ProfileBarItemDto>> GetUsersAsync(
        ProfileQueryRequestDto request,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<ProfileBarItemDto>> GetReportsAsync(
        ProfileQueryRequestDto request,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<ProfileBarItemDto>> GetFailsAsync(
        ProfileQueryRequestDto request,
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
    private const string ReportType = "report";
    private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(1);
    private static readonly Regex RsPrefixRegex = new(@"^rs", RegexOptions.Multiline, RegexTimeout);
    private static readonly Regex SplitCamelCaseRegex = new(
        @"(?<=[a-z])([A-Z])",
        RegexOptions.None,
        RegexTimeout
    );

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

    private sealed class ProfileQueryOptions
    {
        public int Id { get; init; }
        public string Type { get; init; }
        public double StartAt { get; init; }
        public double EndAt { get; init; }
        public List<string> Server { get; init; } = new();
        public List<string> Database { get; init; } = new();
        public List<string> MasterFile { get; init; } = new();
        public List<string> Visible { get; init; } = new();
        public List<string> Certification { get; init; } = new();
        public List<string> Availability { get; init; } = new();
        public List<int> ReportType { get; init; } = new();
    }

    private sealed class ProfileSubqueryResult
    {
        public IQueryable<ProfileRunRow> Flattened { get; init; }
        public IQueryable<IGrouping<DateTime, ProfileRunRow>> Grouped { get; init; }
        public string DateFormat { get; init; }
    }

    private readonly Atlas_WebContext _context;
    private readonly IConfiguration _config;

    public ProfileApiService(Atlas_WebContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<ProfileChartResponseDto> GetChartAsync(
        ProfileQueryRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var query = ToQueryOptions(request);
        var subqueryResult = await BuildSubqueriesAsync(query, cancellationToken);
        var subquery = subqueryResult.Flattened;
        var subqueryGroup = subqueryResult.Grouped;
        var dateFormat = subqueryResult.DateFormat;

        var history = await (
            from grp in subqueryGroup
            orderby grp.Key
            select new ProfileRunHistoryPointDto
            {
                Date = grp.Key.ToString(dateFormat),
                Users = grp.Select(x => x.RunUserId).Distinct().Count(),
                Runs = grp.Sum(x => x.Runs),
                RunTime = Math.Round(grp.Average(x => x.RunDurationSeconds), 1),
            }
        )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var totalRuns = history.Sum(x => x.Runs);
        var distinctUsers = await subquery.Select(x => x.RunUserId).Distinct().CountAsync(cancellationToken);
        var averageRunTime = totalRuns > 0
            ? Math.Round(await subquery.AverageAsync(x => x.RunDurationSeconds, cancellationToken), 2)
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
        ProfileQueryRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var subquery = (await BuildSubqueriesAsync(ToQueryOptions(request), cancellationToken)).Flattened;

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
        ProfileQueryRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var subquery = (await BuildSubqueriesAsync(ToQueryOptions(request), cancellationToken)).Flattened;

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
        ProfileQueryRequestDto request,
        CancellationToken cancellationToken
    )
    {
        var subquery = (await BuildSubqueriesAsync(ToQueryOptions(request), cancellationToken)).Flattened;

        var total = await subquery.SumAsync(x => x.Runs, cancellationToken);
        return await (
            from a in subquery
            where a.RunStatus != "Success"
            group a by a.RunStatus into grp
            select new ProfileBarItemDto
            {
                Key = SplitCamelCaseRegex.Replace(
                    RsPrefixRegex.Replace(grp.Key, string.Empty),
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

        if (string.Equals(type, ReportType, StringComparison.OrdinalIgnoreCase))
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
            !string.Equals(type, ReportType, StringComparison.OrdinalIgnoreCase)
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

    private async Task<ProfileSubqueryResult> BuildSubqueriesAsync(
        ProfileQueryOptions query,
        CancellationToken cancellationToken
    )
    {
        var start = DateTime.Now.AddSeconds(query.StartAt);
        var end = DateTime.Now.AddSeconds(query.EndAt);

        var runData = _context.ReportObjectRunDatas.AsQueryable();
        var reports = _context.ReportObjects.AsQueryable();

        (runData, reports) = await ApplyProfileScopeAsync(
            runData,
            reports,
            query,
            cancellationToken
        );

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

        var (grouped, dateFormat) = BuildDateGrouping(joined, query, start, end);

        var flattened = grouped.SelectMany(x => x);
        return new ProfileSubqueryResult
        {
            Flattened = flattened,
            Grouped = grouped,
            DateFormat = dateFormat,
        };
    }

    private static IQueryable<ReportObject> ApplyReportFilters(
        IQueryable<ReportObject> reports,
        ProfileQueryOptions query
    )
    {
        if (query.Server.Count > 0)
        {
            reports = reports.Where(x => query.Server.Contains(x.SourceServer));
        }

        if (query.Database.Count > 0)
        {
            reports = reports.Where(x => query.Database.Contains(x.SourceDb));
        }

        if (query.MasterFile.Count > 0)
        {
            reports = reports.Where(x =>
                query.MasterFile.Contains(x.EpicMasterFile)
                || (query.MasterFile.Contains("None") && string.IsNullOrEmpty(x.EpicMasterFile))
            );
        }

        if (query.Visible.Count > 0)
        {
            reports = reports.Where(x =>
                query.Visible.Contains(x.DefaultVisibilityYn)
                || (query.Visible.Contains("Y") && string.IsNullOrEmpty(x.DefaultVisibilityYn))
            );
        }

        if (query.Certification.Count > 0)
        {
            reports = reports.Where(x =>
                query.Certification.Intersect(x.ReportTagLinks.Select(y => y.Tag.Name)).Any()
            );
        }

        if (query.Availability.Count > 0)
        {
            reports = reports.Where(x =>
                query.Availability.Contains(x.Availability)
                || (query.Availability.Contains("Public") && string.IsNullOrEmpty(x.Availability))
            );
        }

        if (query.ReportType.Count > 0)
        {
            reports = reports.Where(x => query.ReportType.Contains((int)x.ReportObjectTypeId));
        }

        return reports;
    }

    private bool IsUserProfileEnabled()
    {
        return _config["features:enable_user_profile"] == null
            || string.Equals(
                _config["features:enable_user_profile"],
                bool.TrueString,
                StringComparison.OrdinalIgnoreCase
            );
    }

    private static InvalidOperationException InvalidType(string type, int id)
    {
        return new InvalidOperationException(
            "Wrong parameter value supplied. Type: " + type + " with Id: " + id
        );
    }

    private static ProfileQueryOptions ToQueryOptions(ProfileQueryRequestDto request)
    {
        return new ProfileQueryOptions
        {
            Id = request.Id,
            Type = request.Type,
            StartAt = request.StartAt,
            EndAt = request.EndAt,
            Server = request.Server ?? new List<string>(),
            Database = request.Database ?? new List<string>(),
            MasterFile = request.MasterFile ?? new List<string>(),
            Visible = request.Visible ?? new List<string>(),
            Certification = request.Certification ?? new List<string>(),
            Availability = request.Availability ?? new List<string>(),
            ReportType = request.ReportType ?? new List<int>(),
        };
    }

    private async Task<(IQueryable<ReportObjectRunData> RunData, IQueryable<ReportObject> Reports)> ApplyProfileScopeAsync(
        IQueryable<ReportObjectRunData> runData,
        IQueryable<ReportObject> reports,
        ProfileQueryOptions query,
        CancellationToken cancellationToken
    )
    {
        if (string.Equals(query.Type, ReportType, StringComparison.OrdinalIgnoreCase))
        {
            return await ApplyReportScopeAsync(runData, reports, query, cancellationToken);
        }

        if (string.Equals(query.Type, "term", StringComparison.OrdinalIgnoreCase))
        {
            return await ApplyTermScopeAsync(runData, reports, query, cancellationToken);
        }

        if (string.Equals(query.Type, "collection", StringComparison.OrdinalIgnoreCase))
        {
            return await ApplyCollectionScopeAsync(runData, reports, query, cancellationToken);
        }

        if (string.Equals(query.Type, "user", StringComparison.OrdinalIgnoreCase))
        {
            return await ApplyUserScopeAsync(runData, reports, query, cancellationToken);
        }

        if (string.Equals(query.Type, "group", StringComparison.OrdinalIgnoreCase))
        {
            return await ApplyGroupScopeAsync(runData, reports, query, cancellationToken);
        }

        throw InvalidType(query.Type, query.Id);
    }

    private async Task<(IQueryable<ReportObjectRunData> RunData, IQueryable<ReportObject> Reports)> ApplyReportScopeAsync(
        IQueryable<ReportObjectRunData> runData,
        IQueryable<ReportObject> reports,
        ProfileQueryOptions query,
        CancellationToken cancellationToken
    )
    {
        if (query.Id == -1)
        {
            return (runData, ApplyReportFilters(reports, query));
        }

        if (await _context.ReportObjects.AnyAsync(x => x.ReportObjectId == query.Id, cancellationToken))
        {
            return (runData, reports.Where(x => x.ReportObjectId == query.Id));
        }

        throw InvalidType(query.Type, query.Id);
    }

    private async Task<(IQueryable<ReportObjectRunData> RunData, IQueryable<ReportObject> Reports)> ApplyTermScopeAsync(
        IQueryable<ReportObjectRunData> runData,
        IQueryable<ReportObject> reports,
        ProfileQueryOptions query,
        CancellationToken cancellationToken
    )
    {
        if (!await _context.Terms.AnyAsync(x => x.TermId == query.Id, cancellationToken))
        {
            throw InvalidType(query.Type, query.Id);
        }

        reports = reports.Where(x =>
            _context.ReportObjectDocTerms.Where(t => t.TermId == query.Id)
                .Select(t => t.ReportObjectId)
                .Contains(x.ReportObjectId)
        );

        return (runData, reports);
    }

    private async Task<(IQueryable<ReportObjectRunData> RunData, IQueryable<ReportObject> Reports)> ApplyCollectionScopeAsync(
        IQueryable<ReportObjectRunData> runData,
        IQueryable<ReportObject> reports,
        ProfileQueryOptions query,
        CancellationToken cancellationToken
    )
    {
        if (!await _context.Collections.AnyAsync(x => x.CollectionId == query.Id, cancellationToken))
        {
            throw InvalidType(query.Type, query.Id);
        }

        reports = reports.Where(x =>
            _context.CollectionReports.Where(c => c.CollectionId == query.Id)
                .Select(c => c.ReportId)
                .Contains(x.ReportObjectId)
        );

        return (runData, reports);
    }

    private async Task<(IQueryable<ReportObjectRunData> RunData, IQueryable<ReportObject> Reports)> ApplyUserScopeAsync(
        IQueryable<ReportObjectRunData> runData,
        IQueryable<ReportObject> reports,
        ProfileQueryOptions query,
        CancellationToken cancellationToken
    )
    {
        if (!await _context.Users.AnyAsync(x => x.UserId == query.Id, cancellationToken))
        {
            throw InvalidType(query.Type, query.Id);
        }

        runData = runData.Where(x => x.RunUserId == query.Id);
        return (runData, ApplyReportFilters(reports, query));
    }

    private async Task<(IQueryable<ReportObjectRunData> RunData, IQueryable<ReportObject> Reports)> ApplyGroupScopeAsync(
        IQueryable<ReportObjectRunData> runData,
        IQueryable<ReportObject> reports,
        ProfileQueryOptions query,
        CancellationToken cancellationToken
    )
    {
        if (!await _context.UserGroups.AnyAsync(x => x.GroupId == query.Id, cancellationToken))
        {
            throw InvalidType(query.Type, query.Id);
        }

        runData = runData.Where(x => x.RunUser.UserGroupsMemberships.Any(g => g.GroupId == query.Id));
        return (runData, ApplyReportFilters(reports, query));
    }

    private static (
        IQueryable<IGrouping<DateTime, ProfileRunRow>> Grouped,
        string DateFormat
    ) BuildDateGrouping(
        IQueryable<ProfileRunRow> joined,
        ProfileQueryOptions query,
        DateTime start,
        DateTime end
    )
    {
        var range = query.EndAt - query.StartAt;

        if (range < 172800)
        {
            return (
                joined.Where(x => x.RunStartTime_Hour >= start && x.RunStartTime_Hour <= end)
                    .GroupBy(x => x.RunStartTime_Hour),
                "h tt"
            );
        }

        if (range < 31536000)
        {
            return (
                joined.Where(x => x.RunStartTime_Day >= start && x.RunStartTime_Day <= end)
                    .GroupBy(x => x.RunStartTime_Day),
                range < 691200 ? "ddd M/d" : "MMM d"
            );
        }

        return (
            joined.Where(x => x.RunStartTime_Month >= start && x.RunStartTime_Month <= end)
                .GroupBy(x => x.RunStartTime_Month),
            "MMM yy"
        );
    }
}
