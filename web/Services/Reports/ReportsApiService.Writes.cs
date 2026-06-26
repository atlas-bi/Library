using Atlas_Web.Authorization;
using Atlas_Web.Contracts.Api.Reports;
using Atlas_Web.Models;
using Atlas_Web.Pages.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SolrNet;
using SolrNet.Commands.Parameters;
using System.Security.Claims;

namespace Atlas_Web.Services;

public sealed partial class ReportsApiService
{
    private List<ReportSearchResultDto> SearchObjects(string search, string handler)
    {
        var queryString = IndexModel.BuildSearchString(
            search ?? string.Empty,
            _httpContextAccessor.HttpContext?.Request.Query ?? new QueryCollection()
        );

        return _solr
            .Query(
                new SolrQuery(queryString),
                new QueryOptions
                {
                    RequestHandler = new RequestHandlerParameters(handler),
                    StartOrCursor = new StartOrCursor.Start(0),
                    Rows = 10,
                }
            )
            .Select(x => new ReportSearchResultDto
            {
                Id = x.AtlasId,
                Name = x.Name,
                Description = x.Description != null ? x.Description.FirstOrDefault() : string.Empty,
            })
            .ToList();
    }

    private async Task SynchronizeTermsAsync(
        int reportId,
        IReadOnlyList<int> termIds,
        CancellationToken cancellationToken
    )
    {
        var normalizedIds = termIds.Distinct().ToList();
        var existing = await _context.ReportObjectDocTerms.Where(x => x.ReportObjectId == reportId)
            .ToListAsync(cancellationToken);

        foreach (var termId in normalizedIds.Where(termId => existing.All(x => x.TermId != termId)))
        {
            await _context.ReportObjectDocTerms.AddAsync(
                new ReportObjectDocTerm { ReportObjectId = reportId, TermId = termId },
                cancellationToken
            );
        }

        _context.ReportObjectDocTerms.RemoveRange(
            existing.Where(x => !normalizedIds.Contains(x.TermId))
        );
    }

    private async Task SynchronizeCollectionsAsync(
        int reportId,
        IReadOnlyList<int> collectionIds,
        CancellationToken cancellationToken
    )
    {
        var normalizedIds = collectionIds.Distinct().ToList();
        var existing = await _context.CollectionReports.Where(x => x.ReportId == reportId)
            .ToListAsync(cancellationToken);

        foreach (var link in existing.Where(x => !normalizedIds.Contains(x.CollectionId)))
        {
            _context.CollectionReports.Remove(link);
        }

        for (var index = 0; index < normalizedIds.Count; index++)
        {
            var collectionId = normalizedIds[index];
            var existingLink = existing.FirstOrDefault(x => x.CollectionId == collectionId);
            if (existingLink == null)
            {
                await _context.CollectionReports.AddAsync(
                    new CollectionReport
                    {
                        ReportId = reportId,
                        CollectionId = collectionId,
                        Rank = index,
                    },
                    cancellationToken
                );
            }
            else
            {
                existingLink.Rank = index;
            }
        }
    }

    private async Task SynchronizeFragilityTagsAsync(
        int reportId,
        IReadOnlyList<int> fragilityTagIds,
        CancellationToken cancellationToken
    )
    {
        var normalizedIds = fragilityTagIds.Distinct().ToList();
        var existing = await _context.ReportObjectDocFragilityTags
            .Where(x => x.ReportObjectId == reportId)
            .ToListAsync(cancellationToken);

        foreach (var fragilityTagId in normalizedIds.Where(id => existing.All(x => x.FragilityTagId != id)))
        {
            await _context.ReportObjectDocFragilityTags.AddAsync(
                new ReportObjectDocFragilityTag
                {
                    ReportObjectId = reportId,
                    FragilityTagId = fragilityTagId,
                },
                cancellationToken
            );
        }

        _context.ReportObjectDocFragilityTags.RemoveRange(
            existing.Where(x => !normalizedIds.Contains(x.FragilityTagId))
        );
    }

    private async Task SynchronizeImagesAsync(
        int reportId,
        IReadOnlyList<int> imageIds,
        CancellationToken cancellationToken
    )
    {
        var normalizedIds = imageIds.Distinct().ToList();
        var existing = await _context.ReportObjectImagesDocs.Where(x => x.ReportObjectId == reportId)
            .ToListAsync(cancellationToken);

        _context.ReportObjectImagesDocs.RemoveRange(
            existing.Where(x => !normalizedIds.Contains(x.ImageId))
        );

        for (var index = 0; index < normalizedIds.Count; index++)
        {
            var image = existing.FirstOrDefault(x => x.ImageId == normalizedIds[index]);
            if (image != null)
            {
                image.ImageOrdinal = index;
            }
        }
    }

    private async Task SynchronizeServiceRequestsAsync(
        int reportId,
        IReadOnlyList<int> serviceRequestIds,
        CancellationToken cancellationToken
    )
    {
        var normalizedIds = serviceRequestIds.Distinct().ToList();
        var existing = await _context.ReportServiceRequests.Where(x => x.ReportObjectId == reportId)
            .ToListAsync(cancellationToken);

        _context.ReportServiceRequests.RemoveRange(
            existing.Where(x => !normalizedIds.Contains(x.ServiceRequestId))
        );
    }

    private async Task AddServiceRequestAsync(
        int reportId,
        NewReportServiceRequestDto request,
        CancellationToken cancellationToken
    )
    {
        if (request == null || string.IsNullOrWhiteSpace(request.TicketNumber))
        {
            return;
        }

        await _context.ReportServiceRequests.AddAsync(
            new ReportServiceRequest
            {
                ReportObjectId = reportId,
                TicketNumber = request.TicketNumber,
                Description = request.Description,
                TicketUrl = request.TicketUrl,
            },
            cancellationToken
        );
    }

    private async Task AddMaintenanceLogAsync(
        ClaimsPrincipal user,
        int reportId,
        NewMaintenanceLogDto request,
        CancellationToken cancellationToken
    )
    {
        if (request?.MaintenanceLogStatusId == null)
        {
            return;
        }

        await _context.AddAsync(
            new MaintenanceLog
            {
                ReportId = reportId,
                MaintainerId = user.GetUserId(),
                MaintenanceDate = DateTime.UtcNow,
                MaintenanceLogStatusId = request.MaintenanceLogStatusId,
                Comment = request.Comment,
            },
            cancellationToken
        );
    }

    private static void ValidateImageUpload(IFormFile file)
    {
        var contentType = file.ContentType.ToLowerInvariant();
        if (
            contentType != "image/jpeg"
            && contentType != "image/png"
            && contentType != "image/gif"
        )
        {
            throw new InvalidOperationException("You may only upload jpeg, png or gif files.");
        }

        if (file.Length > 1024 * 1024)
        {
            throw new InvalidOperationException(
                "The file is larger than 1MB. Please use a smaller image."
            );
        }
    }
}
