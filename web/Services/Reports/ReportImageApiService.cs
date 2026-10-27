using System.Text.RegularExpressions;
using Atlas_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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
    private static readonly Regex BothDimensions = new(
        @"^\d+x\d+$",
        RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(100)
    );
    private static readonly Regex WidthOnly = new(
        @"^\d+x_$",
        RegexOptions.CultureInvariant,
        TimeSpan.FromMilliseconds(100)
    );

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

        var encoded = BuildImage(imageData, size, "jpeg");
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
        return EncodeResult(BuildImage(imageData, size, extension), extension);
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

    private static byte[] BuildImage(byte[] imageData, string size, string extension)
    {
        using Image image = Image.Load<Rgba32>(imageData);
        Resize(image, size);

        using var stream = new MemoryStream();
        if (extension == "webp")
        {
            image.Save(stream, new WebpEncoder { Quality = 75 });
        }
        else
        {
            image.Save(stream, new JpegEncoder { Quality = 75 });
        }

        return stream.ToArray();
    }

    private static void Resize(Image image, string size)
    {
        if (string.IsNullOrEmpty(size) || image.Width <= 0 || image.Height <= 0)
        {
            return;
        }

        if (BothDimensions.IsMatch(size))
        {
            var parts = size.Split('x');
            if (!int.TryParse(parts[0], out var width) || !int.TryParse(parts[1], out var height))
            {
                return;
            }

            var maxRatio = Math.Max(width / (float)image.Width, height / (float)image.Height);
            if (maxRatio < 1)
            {
                image.Mutate(x =>
                    x.Resize((int)(image.Width * maxRatio), (int)(image.Height * maxRatio))
                );
            }

            return;
        }

        if (!WidthOnly.IsMatch(size))
        {
            return;
        }

        var widthPart = size.Split('x')[0];
        if (!int.TryParse(widthPart, out var targetWidth))
        {
            return;
        }

        var widthRatio = targetWidth / (float)image.Width;
        if (widthRatio < 1)
        {
            image.Mutate(x =>
                x.Resize((int)(image.Width * widthRatio), (int)(image.Height * widthRatio))
            );
        }
    }
}
