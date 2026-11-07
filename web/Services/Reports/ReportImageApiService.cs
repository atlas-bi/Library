using Atlas_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Atlas_Web.Services;

public interface IReportImageApiService
{
    Task<ReportImageResult> GetThumbnailAsync(
        int reportId,
        string size,
        int? imageId,
        string type,
        CancellationToken cancellationToken
    );

    Task<ReportImageResult> GetPlaceholderAsync(string size, CancellationToken cancellationToken);
}

public sealed class ReportImageResult
{
    public ReportImageStatus Status { get; init; }
    public byte[] Bytes { get; init; }
    public string ContentType { get; init; }

    public static ReportImageResult NotFound()
    {
        return new ReportImageResult { Status = ReportImageStatus.NotFound };
    }
}

public enum ReportImageStatus
{
    Ok,
    NotFound,
}

public static class ReportImageRequests
{
    public const string SessionCookieName = "atlas_token";

    public static bool AcceptsSessionCookie(HttpRequest request)
    {
        if (!HttpMethods.IsGet(request.Method))
        {
            return false;
        }

        var path = request.Path.Value ?? string.Empty;
        return path.EndsWith("/thumbnail", StringComparison.OrdinalIgnoreCase)
            || path.EndsWith("/images/placeholder", StringComparison.OrdinalIgnoreCase);
    }
}

public sealed class ReportImageApiService : IReportImageApiService
{
    private readonly Atlas_WebContext _context;
    private readonly IMemoryCache _cache;
    private readonly string _contentRoot;

    public ReportImageApiService(Atlas_WebContext context, IMemoryCache cache, string contentRoot)
    {
        _context = context;
        _cache = cache;
        _contentRoot = contentRoot;
    }

    public async Task<ReportImageResult> GetThumbnailAsync(
        int reportId,
        string size,
        int? imageId,
        string type,
        CancellationToken cancellationToken
    )
    {
        var image = imageId.HasValue
            ? await _context.ReportObjectImagesDocs.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ImageId == imageId.Value, cancellationToken)
            : await _context.ReportObjectImagesDocs.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ReportObjectId == reportId, cancellationToken);

        var imageData = image?.ImageData;
        if (imageData == null || imageData.Length == 0)
        {
            imageData = await ReadPlaceholderBytesAsync(cancellationToken);
        }

        if (imageData == null)
        {
            return ReportImageResult.NotFound();
        }

        var extension = type ?? "jpeg";
        return Encode(imageData, size, extension);
    }

    public async Task<ReportImageResult> GetPlaceholderAsync(
        string size,
        CancellationToken cancellationToken
    )
    {
        var cacheKey = "report-image-placeholder:" + (size ?? string.Empty);
        if (_cache.TryGetValue(cacheKey, out byte[] cached) && cached != null)
        {
            return EncodeResult(cached, "jpeg");
        }

        var imageData = await ReadPlaceholderBytesAsync(cancellationToken);
        if (imageData == null)
        {
            return ReportImageResult.NotFound();
        }

        var encoded = ReportThumbnail.Build(imageData, size, "jpeg");
        _cache.Set(cacheKey, encoded, TimeSpan.FromMinutes(20));
        return EncodeResult(encoded, "jpeg");
    }

    private async Task<byte[]> ReadPlaceholderBytesAsync(CancellationToken cancellationToken)
    {
        var path = Path.Combine(_contentRoot, "wwwroot", "img", "report_placeholder.png");
        if (!File.Exists(path))
        {
            return null;
        }

        return await File.ReadAllBytesAsync(path, cancellationToken);
    }

    private static ReportImageResult Encode(byte[] imageData, string size, string extension)
    {
        return EncodeResult(ReportThumbnail.Build(imageData, size, extension), extension);
    }

    private static ReportImageResult EncodeResult(byte[] bytes, string extension)
    {
        return new ReportImageResult
        {
            Status = ReportImageStatus.Ok,
            Bytes = bytes,
            ContentType = extension == "webp" ? "image/webp" : "image/jpeg",
        };
    }
}
