using System.Globalization;
using Atlas_Web.Contracts.Api.Analytics;
using Atlas_Web.Helpers;
using Atlas_Web.Models;
using Microsoft.EntityFrameworkCore;
using UAParser;

namespace Atlas_Web.Services;

public interface IAnalyticsApiService
{
    Task<AnalyticsVisitsResponseDto> GetVisitsAsync(
        AnalyticsQueryRequest request,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<AnalyticsBarItemDto>> GetBrowsersAsync(
        AnalyticsQueryRequest request,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<AnalyticsBarItemDto>> GetOsAsync(
        AnalyticsQueryRequest request,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<AnalyticsBarItemDto>> GetResolutionAsync(
        AnalyticsQueryRequest request,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<AnalyticsBarItemDto>> GetUsersAsync(
        AnalyticsQueryRequest request,
        CancellationToken cancellationToken
    );
    Task<IReadOnlyList<AnalyticsBarItemDto>> GetLoadTimesAsync(
        AnalyticsQueryRequest request,
        CancellationToken cancellationToken
    );
    Task<AnalyticsLiveUsersResponseDto> GetLiveUsersAsync(CancellationToken cancellationToken);
    Task RecordBeaconAsync(
        int userId,
        bool isHyperspace,
        AnalyticsBeaconRequest request,
        CancellationToken cancellationToken
    );
    Task<AnalyticsTraceListResponseDto> GetTracesAsync(
        AnalyticsLogQueryRequest request,
        CancellationToken cancellationToken
    );
    Task RecordTracesAsync(
        int userId,
        string userAgent,
        string referer,
        AnalyticsTraceIngestRequest request,
        CancellationToken cancellationToken
    );
    Task ResolveTraceAsync(int id, int type, CancellationToken cancellationToken);
    Task<AnalyticsErrorListResponseDto> GetErrorsAsync(
        AnalyticsLogQueryRequest request,
        CancellationToken cancellationToken
    );
    Task ResolveErrorAsync(int id, int type, CancellationToken cancellationToken);
}

public sealed class AnalyticsApiService : IAnalyticsApiService
{
    private const int LogPageSize = 10;
    private readonly Atlas_WebContext _context;
    private readonly IConfiguration _config;

    public AnalyticsApiService(Atlas_WebContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<AnalyticsVisitsResponseDto> GetVisitsAsync(
        AnalyticsQueryRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = await GetAnalyticsQueryAsync(request, cancellationToken);
        var views = await query.CountAsync(cancellationToken);
        if (views == 0)
        {
            return new AnalyticsVisitsResponseDto();
        }

        var minDate = new DateTime(1900, 1, 1);
        var range = request.EndAt - request.StartAt;
        var accessHistory = range switch
        {
            < 172800 => await (
                from analytic in query
                group analytic by minDate.AddHours(
                    EF.Functions.DateDiffHour(minDate, analytic.AccessDateTime ?? DateTime.Now)
                ) into groupByDate
                orderby groupByDate.Key
                select new AnalyticsAccessHistoryDto
                {
                    Date = groupByDate.Key.ToString("h tt"),
                    Sessions = groupByDate.Select(x => x.SessionId).Distinct().Count(),
                    Pages = groupByDate.Select(x => x.PageId).Distinct().Count(),
                    LoadTime = Math.Round(
                        groupByDate.Average(x => (long)Convert.ToDouble(x.LoadTime)) / 1000,
                        1
                    ),
                }
            ).ToListAsync(cancellationToken),
            < 691200 => await (
                from analytic in query
                group analytic by minDate.AddDays(
                    EF.Functions.DateDiffDay(minDate, analytic.AccessDateTime ?? DateTime.Now)
                ) into groupByDate
                orderby groupByDate.Key
                select new AnalyticsAccessHistoryDto
                {
                    Date = groupByDate.Key.ToString("ddd M/d"),
                    Sessions = groupByDate.Select(x => x.SessionId).Distinct().Count(),
                    Pages = groupByDate.Select(x => x.PageId).Distinct().Count(),
                    LoadTime = Math.Round(
                        groupByDate.Average(x => (long)Convert.ToDouble(x.LoadTime)) / 1000,
                        1
                    ),
                }
            ).ToListAsync(cancellationToken),
            < 31536000 => await (
                from analytic in query
                group analytic by minDate.AddDays(
                    EF.Functions.DateDiffDay(minDate, analytic.AccessDateTime ?? DateTime.Now)
                ) into groupByDate
                orderby groupByDate.Key
                select new AnalyticsAccessHistoryDto
                {
                    Date = groupByDate.Key.ToString("MMM d"),
                    Sessions = groupByDate.Select(x => x.SessionId).Distinct().Count(),
                    Pages = groupByDate.Select(x => x.PageId).Distinct().Count(),
                    LoadTime = Math.Round(
                        groupByDate.Average(x => (long)Convert.ToDouble(x.LoadTime)) / 1000,
                        1
                    ),
                }
            ).ToListAsync(cancellationToken),
            _ => await (
                from analytic in query
                group analytic by minDate.AddMonths(
                    EF.Functions.DateDiffMonth(minDate, analytic.AccessDateTime ?? DateTime.Now)
                ) into groupByDate
                orderby groupByDate.Key
                select new AnalyticsAccessHistoryDto
                {
                    Date = groupByDate.Key.ToString("MMM yy"),
                    Sessions = groupByDate.Select(x => x.SessionId).Distinct().Count(),
                    Pages = groupByDate.Select(x => x.PageId).Distinct().Count(),
                    LoadTime = Math.Round(
                        groupByDate.Average(x => (long)Convert.ToDouble(x.LoadTime)) / 1000,
                        1
                    ),
                }
            ).ToListAsync(cancellationToken),
        };

        return new AnalyticsVisitsResponseDto
        {
            Views = views,
            Visitors = await query.Select(x => x.SessionId).Distinct().CountAsync(cancellationToken),
            LoadTime = Math.Round(
                await query.AverageAsync(x => (long)Convert.ToDouble(x.LoadTime), cancellationToken) / 1000,
                1
            ),
            AccessHistory = accessHistory,
        };
    }

    public async Task<IReadOnlyList<AnalyticsBarItemDto>> GetBrowsersAsync(
        AnalyticsQueryRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = await GetAnalyticsQueryAsync(request, cancellationToken);
        var total = await query.CountAsync(cancellationToken);
        if (total == 0) return Array.Empty<AnalyticsBarItemDto>();

        var parser = Parser.GetDefault();
        var grouped = await query
            .GroupBy(x => x.UserAgent)
            .Select(x => new { x.Key, Count = x.Count() })
            .ToListAsync(cancellationToken);

        return grouped
            .Select(x => new { Parsed = parser.Parse(x.Key ?? ""), x.Count })
            .GroupBy(x => new { x.Parsed.UA.Family, x.Parsed.UA.Major })
            .Select(groupByBrowser => new AnalyticsBarItemDto
            {
                Key = groupByBrowser.Key.Family + " " + groupByBrowser.Key.Major,
                Count = groupByBrowser.Sum(x => x.Count),
                Percent = (double)groupByBrowser.Sum(x => x.Count) / total,
                TitleOne = "Browser",
                TitleTwo = "Views",
            })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToList();
    }

    public async Task<IReadOnlyList<AnalyticsBarItemDto>> GetOsAsync(
        AnalyticsQueryRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = await GetAnalyticsQueryAsync(request, cancellationToken);
        var total = await query.CountAsync(cancellationToken);
        if (total == 0) return Array.Empty<AnalyticsBarItemDto>();

        var parser = Parser.GetDefault();
        var grouped = await query
            .GroupBy(x => x.UserAgent)
            .Select(x => new { x.Key, Count = x.Count() })
            .ToListAsync(cancellationToken);

        return grouped
            .Select(x => new { Parsed = parser.Parse(x.Key ?? ""), x.Count })
            .GroupBy(x => new { x.Parsed.OS.Family, x.Parsed.OS.Major })
            .Select(groupByOs => new AnalyticsBarItemDto
            {
                Key = groupByOs.Key.Family + " " + groupByOs.Key.Major,
                Count = groupByOs.Sum(x => x.Count),
                Percent = (double)groupByOs.Sum(x => x.Count) / total,
                TitleOne = "Operating System",
                TitleTwo = "Views",
            })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToList();
    }

    public async Task<IReadOnlyList<AnalyticsBarItemDto>> GetResolutionAsync(
        AnalyticsQueryRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = await GetAnalyticsQueryAsync(request, cancellationToken);
        var total = await query.CountAsync(cancellationToken);
        if (total == 0) return Array.Empty<AnalyticsBarItemDto>();

        return await (
            from analytic in query
            group analytic by new { analytic.ScreenWidth, analytic.ScreenHeight } into groupByResolution
            select new AnalyticsBarItemDto
            {
                Key = groupByResolution.Key.ScreenWidth + "x" + groupByResolution.Key.ScreenHeight,
                Count = groupByResolution.Count(),
                Percent = (double)groupByResolution.Count() / total,
                TitleOne = "Window Resolution",
                TitleTwo = "Views",
            }
        )
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AnalyticsBarItemDto>> GetUsersAsync(
        AnalyticsQueryRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = await GetAnalyticsQueryAsync(request, cancellationToken);
        var total = await query.CountAsync(cancellationToken);
        if (total == 0) return Array.Empty<AnalyticsBarItemDto>();

        var profileEnabled = _config["features:enable_user_profile"] == null
            || _config["features:enable_user_profile"]!.ToLower() == "true";

        return await (
            from analytic in query
            group analytic by new { analytic.UserId, analytic.User.FullnameCalc } into groupByUser
            select new AnalyticsBarItemDto
            {
                Key = groupByUser.Key.FullnameCalc,
                Count = groupByUser.Count(),
                Percent = (double)groupByUser.Count() / total,
                Href = profileEnabled ? "/users?id=" + groupByUser.Key.UserId : null,
                TitleOne = "Top Users",
                TitleTwo = "Views",
            }
        )
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AnalyticsBarItemDto>> GetLoadTimesAsync(
        AnalyticsQueryRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = await GetAnalyticsQueryAsync(request, cancellationToken);
        return await (
            from analytic in query
            group analytic by analytic.Pathname into groupByPath
            select new AnalyticsBarItemDto
            {
                Key = groupByPath.Key,
                Count = Math.Round(
                    groupByPath.Average(x => (long)Convert.ToDouble(x.LoadTime)) / 1000,
                    1
                ),
                TitleOne = "Load Times",
                TitleTwo = "Seconds",
            }
        )
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync(cancellationToken);
    }

    public async Task<AnalyticsLiveUsersResponseDto> GetLiveUsersAsync(
        CancellationToken cancellationToken
    )
    {
        var activeSince = DateTime.Now.AddSeconds(-60);
        var latest =
            from analytic in _context.Analytics
            where analytic.UpdateTime >= activeSince
            group analytic by new { analytic.UserId, analytic.SessionId } into grouped
            select new
            {
                grouped.Key.UserId,
                grouped.Key.SessionId,
                Time = grouped.Max(x => x.UpdateTime),
                SessionTime = grouped.Sum(x => x.PageTime ?? 0),
                Pages = grouped.Count(),
            };

        var rows = await (
            from analytic in _context.Analytics
            join current in latest
                on new { analytic.UserId, analytic.SessionId, Time = analytic.UpdateTime }
                equals new { current.UserId, current.SessionId, Time = current.Time }
            join user in _context.Users on analytic.UserId equals user.UserId
            select new
            {
                user.FullnameCalc,
                analytic.UserId,
                current.SessionId,
                current.SessionTime,
                analytic.PageTime,
                analytic.Href,
                analytic.AccessDateTime,
                analytic.UpdateTime,
                current.Pages,
            }
        ).ToListAsync(cancellationToken);

        var items = rows.Select(x => new AnalyticsLiveUserDto
        {
            Fullname = x.FullnameCalc,
            UserId = x.UserId,
            SessionId = x.SessionId,
            SessionTime = TimeSpan.FromMilliseconds(x.SessionTime).ToString(@"h\:mm\:ss", CultureInfo.InvariantCulture),
            PageTime = TimeSpan.FromMilliseconds(x.PageTime ?? 0).ToString(@"h\:mm\:ss", CultureInfo.InvariantCulture),
            Href = x.Href,
            AccessDateTime = (x.AccessDateTime ?? DateTime.Now).ToString(@"M/d/yy h\:mm\:ss tt", CultureInfo.InvariantCulture),
            UpdateTime = (x.UpdateTime ?? DateTime.Now).ToString(@"M/d/yy h\:mm\:ss tt", CultureInfo.InvariantCulture),
            Pages = x.Pages,
        }).ToList();

        return new AnalyticsLiveUsersResponseDto
        {
            ActiveUsers = items.Select(x => new { x.UserId, x.SessionId }).Distinct().Count(),
            Items = items,
        };
    }

    public async Task RecordBeaconAsync(
        int userId,
        bool isHyperspace,
        AnalyticsBeaconRequest request,
        CancellationToken cancellationToken
    )
    {
        var existing = await _context.Analytics.FirstOrDefaultAsync(
            x => x.UserId == userId && x.SessionId == request.SessionId && x.PageId == request.PageId,
            cancellationToken
        );
        if (existing != null)
        {
            existing.PageTime = request.PageTime ?? 0;
            existing.UpdateTime = DateTime.Now;
        }
        else
        {
            await _context.Analytics.AddAsync(new Analytic
            {
                UserId = userId,
                Language = request.Language ?? "",
                UserAgent = request.UserAgent ?? "",
                Hostname = request.Hostname ?? "",
                Href = request.Href ?? "",
                Protocol = request.Protocol ?? "",
                Search = request.Search ?? "",
                Pathname = request.Pathname ?? "",
                ScreenHeight = request.ScreenHeight ?? "",
                ScreenWidth = request.ScreenWidth ?? "",
                Origin = request.Origin ?? "",
                LoadTime = request.LoadTime ?? "",
                AccessDateTime = DateTime.Now,
                UpdateTime = DateTime.Now,
                Referrer = request.Referrer ?? "",
                Zoom = request.Zoom ?? 0,
                Epic = isHyperspace ? 1 : 0,
                SessionId = request.SessionId ?? "",
                PageId = request.PageId ?? "",
                PageTime = request.PageTime ?? 0,
            }, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AnalyticsTraceListResponseDto> GetTracesAsync(
        AnalyticsLogQueryRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = await GetTraceQueryAsync(request, cancellationToken);
        var totalCount = await query.CountAsync(cancellationToken);
        var page = Math.Max(request.Page, 0);
        var traceRows = await query
            .OrderByDescending(x => x.LogDateTime)
            .Skip(page * LogPageSize)
            .Take(LogPageSize)
            .ToListAsync(cancellationToken);
        var traceUserNames = await _context.Users
            .Where(x => traceRows.Select(row => row.UserId).Contains(x.UserId))
            .ToDictionaryAsync(x => x.UserId, x => x.FullnameCalc, cancellationToken);
        var items = traceRows.Select(x => new AnalyticsTraceDto
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = traceUserNames.GetValueOrDefault(x.UserId),
                Level = x.Level,
                Message = x.Message,
                Logger = x.Logger,
                LogDateTime = x.LogDateTime,
                Handled = x.Handled,
                UserAgent = x.UserAgent,
                Referer = x.Referer,
            })
            .ToList();

        return new AnalyticsTraceListResponseDto
        {
            Pages = (int)Math.Ceiling(totalCount / (double)LogPageSize),
            CurrentPage = page + 1,
            TotalCount = totalCount,
            UnresolvedCount = await query.CountAsync(x => x.Handled != 1, cancellationToken),
            Items = items,
        };
    }

    public async Task RecordTracesAsync(
        int userId,
        string userAgent,
        string referer,
        AnalyticsTraceIngestRequest request,
        CancellationToken cancellationToken
    )
    {
        foreach (var log in request.Logs ?? Array.Empty<AnalyticsTraceEntryRequest>())
        {
            await _context.AnalyticsTraces.AddAsync(new AnalyticsTrace
            {
                UserId = userId,
                Level = log.Level,
                Message = log.Message,
                Logger = log.Logger,
                LogDateTime = DateTime.Now,
                UserAgent = userAgent,
                Referer = referer,
            }, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task ResolveTraceAsync(int id, int type, CancellationToken cancellationToken)
    {
        var trace = await _context.AnalyticsTraces.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (trace == null) return;

        trace.Handled = type == 1 ? 1 : null;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AnalyticsErrorListResponseDto> GetErrorsAsync(
        AnalyticsLogQueryRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = await GetErrorQueryAsync(request, cancellationToken);
        var totalCount = await query.CountAsync(cancellationToken);
        var page = Math.Max(request.Page, 0);
        var errorRows = await query
            .OrderByDescending(x => x.LogDateTime)
            .Skip(page * LogPageSize)
            .Take(LogPageSize)
            .ToListAsync(cancellationToken);
        var errorUserNames = await _context.Users
            .Where(x => errorRows.Select(row => row.UserId).Contains(x.UserId))
            .ToDictionaryAsync(x => x.UserId, x => x.FullnameCalc, cancellationToken);
        var items = errorRows.Select(x => new AnalyticsErrorDto
            {
                Id = x.Id,
                UserId = x.UserId,
                UserName = errorUserNames.GetValueOrDefault(x.UserId),
                StatusCode = x.StatusCode,
                Message = x.Message,
                Trace = x.Trace,
                LogDateTime = x.LogDateTime,
                Handled = x.Handled,
                UserAgent = x.UserAgent,
                Referrer = x.Referrer,
            })
            .ToList();

        return new AnalyticsErrorListResponseDto
        {
            Pages = (int)Math.Ceiling(totalCount / (double)LogPageSize),
            CurrentPage = page + 1,
            TotalCount = totalCount,
            UnresolvedCount = await query.CountAsync(x => x.Handled != 1, cancellationToken),
            Items = items,
        };
    }

    public async Task ResolveErrorAsync(int id, int type, CancellationToken cancellationToken)
    {
        var error = await _context.AnalyticsErrors.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (error == null) return;

        error.Handled = type == 1 ? 1 : null;
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task<IQueryable<Analytic>> GetAnalyticsQueryAsync(
        AnalyticsQueryRequest request,
        CancellationToken cancellationToken
    )
    {
        var now = DateTime.Now;
        var query = _context.Analytics.Where(x =>
            x.AccessDateTime >= now.AddSeconds(request.StartAt)
            && x.AccessDateTime <= now.AddSeconds(request.EndAt)
        );

        if (request.UserId > 0 && await _context.Users.AnyAsync(x => x.UserId == request.UserId, cancellationToken))
        {
            query = query.Where(x => x.UserId == request.UserId);
        }

        if (request.GroupId > 0 && await _context.UserGroups.AnyAsync(x => x.GroupId == request.GroupId, cancellationToken))
        {
            query = query.Where(x => x.User.UserGroupsMemberships.Any(y => y.GroupId == request.GroupId));
        }

        return query;
    }

    private async Task<IQueryable<AnalyticsTrace>> GetTraceQueryAsync(
        AnalyticsLogQueryRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = _context.AnalyticsTraces.Where(x =>
            x.UserAgent != null
            && x.LogDateTime >= DateTime.Now.AddSeconds(request.StartAt)
            && x.LogDateTime <= DateTime.Now.AddSeconds(request.EndAt)
        );

        if (request.UserId > 0 && await _context.Users.AnyAsync(x => x.UserId == request.UserId, cancellationToken))
        {
            query = query.Where(x => x.UserId == request.UserId);
        }

        if (request.GroupId > 0 && await _context.UserGroups.AnyAsync(x => x.GroupId == request.GroupId, cancellationToken))
        {
            query = query.Where(x => x.User.UserGroupsMemberships.Any(y => y.GroupId == request.GroupId));
        }

        return query;
    }

    private async Task<IQueryable<AnalyticsError>> GetErrorQueryAsync(
        AnalyticsLogQueryRequest request,
        CancellationToken cancellationToken
    )
    {
        var query = _context.AnalyticsErrors.Where(x =>
            x.UserAgent != null
            && x.LogDateTime >= DateTime.Now.AddSeconds(request.StartAt)
            && x.LogDateTime <= DateTime.Now.AddSeconds(request.EndAt)
        );

        if (request.UserId > 0 && await _context.Users.AnyAsync(x => x.UserId == request.UserId, cancellationToken))
        {
            query = query.Where(x => x.UserId == request.UserId);
        }

        if (request.GroupId > 0 && await _context.UserGroups.AnyAsync(x => x.GroupId == request.GroupId, cancellationToken))
        {
            query = query.Where(x => x.User.UserGroupsMemberships.Any(y => y.GroupId == request.GroupId));
        }

        return query;
    }
}
