using System.Text.RegularExpressions;
using SkiaSharp;

namespace Atlas_Web.Services;

public static class ReportThumbnail
{
    private const int Quality = 75;

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

    public static byte[] Build(byte[] imageData, string size, string extension)
    {
        using var codec =
            SKCodec.Create(new MemoryStream(imageData))
            ?? throw new InvalidDataException("Image data could not be decoded.");
        using var source = SKBitmap.Decode(codec);
        using var image = SKImage.FromBitmap(source);

        var ratio = ShrinkRatio(source.Width, source.Height, size);
        var width = Math.Max(1, (int)(source.Width * ratio));
        var height = Math.Max(1, (int)(source.Height * ratio));
        var isWebp = extension == "webp";

        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        surface.Canvas.Clear(isWebp ? SKColors.Transparent : SKColors.White);
        surface.Canvas.DrawImage(
            image,
            SKRect.Create(width, height),
            new SKSamplingOptions(SKCubicResampler.Mitchell)
        );

        using var result = surface.Snapshot();
        using var encoded = result.Encode(
            isWebp ? SKEncodedImageFormat.Webp : SKEncodedImageFormat.Jpeg,
            Quality
        );
        return encoded.ToArray();
    }

    private static float ShrinkRatio(int width, int height, string size)
    {
        if (string.IsNullOrEmpty(size) || width <= 0 || height <= 0)
        {
            return 1;
        }

        var parts = size.Split('x');
        if (BothDimensions.IsMatch(size))
        {
            return int.TryParse(parts[0], out var targetWidth)
                && int.TryParse(parts[1], out var targetHeight)
                ? Math.Min(1, Math.Max(targetWidth / (float)width, targetHeight / (float)height))
                : 1;
        }

        return WidthOnly.IsMatch(size) && int.TryParse(parts[0], out var widthOnly)
            ? Math.Min(1, widthOnly / (float)width)
            : 1;
    }
}
