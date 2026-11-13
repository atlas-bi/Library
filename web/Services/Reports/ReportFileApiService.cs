using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Atlas_Web.Services;

public interface IReportFileApiService
{
    Task<ReportFileResult> GetCubeAsync(int reportId, CancellationToken cancellationToken);
    Task<ReportFileResult> GetCrystalAttachmentAsync(
        ClaimsPrincipal user,
        int attachmentId,
        CancellationToken cancellationToken
    );
}

public sealed class ReportFileResult
{
    public ReportFileStatus Status { get; init; }
    public byte[] Bytes { get; init; }
    public string ContentType { get; init; }
    public string FileName { get; init; }
    public bool Inline { get; init; }

    public static ReportFileResult NotFound()
    {
        return new ReportFileResult { Status = ReportFileStatus.NotFound };
    }

    public static ReportFileResult Forbidden()
    {
        return new ReportFileResult { Status = ReportFileStatus.Forbidden };
    }
}

public enum ReportFileStatus
{
    Ok,
    NotFound,
    Forbidden,
}

public sealed class ReportFileApiService : IReportFileApiService
{
    private readonly Atlas_WebContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly string _contentRoot;

    public ReportFileApiService(
        Atlas_WebContext context,
        IAuthorizationService authorizationService,
        string contentRoot
    )
    {
        _context = context;
        _authorizationService = authorizationService;
        _contentRoot = contentRoot;
    }

    public async Task<ReportFileResult> GetCubeAsync(int reportId, CancellationToken cancellationToken)
    {
        var report = await _context.ReportObjects.AsNoTracking()
            .FirstOrDefaultAsync(x => x.ReportObjectId == reportId, cancellationToken);
        if (report == null)
        {
            return ReportFileResult.NotFound();
        }

        var templatePath = Path.Combine(_contentRoot, "wwwroot", "defaults", "Cube.xml");
        var template = await File.ReadAllTextAsync(templatePath, cancellationToken);
        var text = template
            .Replace("server", report.SourceServer)
            .Replace("Catalog_Name", report.Name);

        return new ReportFileResult
        {
            Status = ReportFileStatus.Ok,
            Bytes = Encoding.ASCII.GetBytes(text),
            ContentType = "text/plain",
            FileName = report.Name + ".odc",
        };
    }

    public async Task<ReportFileResult> GetCrystalAttachmentAsync(
        ClaimsPrincipal user,
        int attachmentId,
        CancellationToken cancellationToken
    )
    {
        var attachment = await _context.ReportObjectAttachments
            .Include(x => x.ReportObject)
            .ThenInclude(x => x.ReportObjectType)
            .Include(x => x.ReportObject)
            .ThenInclude(x => x.ReportGroupsMemberships)
            .FirstOrDefaultAsync(x => x.ReportObjectAttachmentId == attachmentId, cancellationToken);
        if (attachment?.ReportObject == null)
        {
            return ReportFileResult.NotFound();
        }

        var authorization = await _authorizationService.AuthorizeAsync(
            user,
            attachment.ReportObject,
            "ReportRunPolicy"
        );
        if (!authorization.Succeeded)
        {
            return ReportFileResult.Forbidden();
        }

        if (string.IsNullOrWhiteSpace(attachment.Path) || !File.Exists(attachment.Path))
        {
            return ReportFileResult.NotFound();
        }

        return new ReportFileResult
        {
            Status = ReportFileStatus.Ok,
            Bytes = await File.ReadAllBytesAsync(attachment.Path, cancellationToken),
            ContentType = "application/pdf",
            FileName = attachment.Name,
            Inline = true,
        };
    }

    public static string BuildContentDisposition(string fileName)
    {
        return new ContentDisposition { FileName = fileName, Inline = true }.ToString();
    }
}
