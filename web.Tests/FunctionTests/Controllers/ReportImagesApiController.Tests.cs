using System.IO;
using System.Threading.Tasks;
using Atlas_Web.Controllers.Api;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkiaSharp;
using Xunit;

namespace web.Tests.FunctionTests.Controllers;

public class ReportImagesApiControllerTests
{
    [Fact]
    public async Task GetThumbnail_ShrinksStoredImage()
    {
        var root = CreateRoot();
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase("report-images-shrink")
            .Options;
        await using var context = new Atlas_WebContext(options);
        context.ReportObjectImagesDocs.Add(
            new ReportObjectImagesDoc
            {
                ImageId = 3,
                ReportObjectId = 9,
                ImageData = Png(80, 40),
            }
        );
        await context.SaveChangesAsync();

        var controller = BuildController(context, root);
        var result = await controller.GetThumbnail(9, "20x20", null, null);

        var file = Assert.IsType<Microsoft.AspNetCore.Mvc.FileContentResult>(result);
        Assert.Equal("image/jpeg", file.ContentType);
        using var image = SKBitmap.Decode(file.FileContents);
        Assert.Equal(40, image.Width);
        Assert.Equal(20, image.Height);
    }

    [Fact]
    public async Task GetThumbnail_UsesRequestedImage()
    {
        var root = CreateRoot();
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase("report-images-id")
            .Options;
        await using var context = new Atlas_WebContext(options);
        context.ReportObjectImagesDocs.AddRange(
            new ReportObjectImagesDoc
            {
                ImageId = 1,
                ReportObjectId = 4,
                ImageData = Png(10, 10),
            },
            new ReportObjectImagesDoc
            {
                ImageId = 2,
                ReportObjectId = 4,
                ImageData = Png(100, 50),
            }
        );
        await context.SaveChangesAsync();

        var controller = BuildController(context, root);
        var result = await controller.GetThumbnail(4, "25x25", 2, "jpeg");

        var file = Assert.IsType<Microsoft.AspNetCore.Mvc.FileContentResult>(result);
        using var image = SKBitmap.Decode(file.FileContents);
        Assert.Equal(50, image.Width);
        Assert.Equal(25, image.Height);
    }

    [Fact]
    public async Task GetThumbnail_UsesPlaceholderWhenReportHasNoImage()
    {
        var root = CreateRoot();
        await File.WriteAllBytesAsync(PlaceholderPath(root), Png(30, 30));
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase("report-images-missing")
            .Options;
        await using var context = new Atlas_WebContext(options);

        var controller = BuildController(context, root);
        var result = await controller.GetThumbnail(99, "10x10", null, null);

        var file = Assert.IsType<Microsoft.AspNetCore.Mvc.FileContentResult>(result);
        Assert.Equal("image/jpeg", file.ContentType);
        Assert.NotEmpty(file.FileContents);
    }

    [Fact]
    public async Task GetThumbnail_ReturnsNotFoundWhenPlaceholderFileIsMissing()
    {
        var root = CreateRoot();
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase("report-images-no-file")
            .Options;
        await using var context = new Atlas_WebContext(options);
        var controller = BuildController(context, root);

        var result = await controller.GetThumbnail(1, "10x10", null, null);

        Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result);
    }

    [Fact]
    public async Task GetPlaceholder_CachesEncodedJpeg()
    {
        var root = CreateRoot();
        var path = PlaceholderPath(root);
        await File.WriteAllBytesAsync(path, Png(30, 30));
        var options = new DbContextOptionsBuilder<Atlas_WebContext>()
            .UseInMemoryDatabase("report-images-placeholder")
            .Options;
        await using var context = new Atlas_WebContext(options);
        var controller = BuildController(context, root);

        var first = await controller.GetPlaceholder("16x16");
        File.Delete(path);
        var second = await controller.GetPlaceholder("16x16");

        var firstFile = Assert.IsType<Microsoft.AspNetCore.Mvc.FileContentResult>(first);
        var secondFile = Assert.IsType<Microsoft.AspNetCore.Mvc.FileContentResult>(second);
        Assert.Equal(firstFile.FileContents, secondFile.FileContents);
    }

    [Fact]
    public void AcceptsSessionCookie_OnlyForReportImageGets()
    {
        var thumbnail = new DefaultHttpContext();
        thumbnail.Request.Method = HttpMethods.Get;
        thumbnail.Request.Path = "/api/reports/9/thumbnail";

        var placeholder = new DefaultHttpContext();
        placeholder.Request.Method = HttpMethods.Get;
        placeholder.Request.Path = "/api/reports/images/placeholder";

        var post = new DefaultHttpContext();
        post.Request.Method = HttpMethods.Post;
        post.Request.Path = "/api/reports/9/thumbnail";

        var other = new DefaultHttpContext();
        other.Request.Method = HttpMethods.Get;
        other.Request.Path = "/api/reports/9";

        Assert.True(ReportImageRequests.AcceptsSessionCookie(thumbnail.Request));
        Assert.True(ReportImageRequests.AcceptsSessionCookie(placeholder.Request));
        Assert.False(ReportImageRequests.AcceptsSessionCookie(post.Request));
        Assert.False(ReportImageRequests.AcceptsSessionCookie(other.Request));
    }

    private static ReportImagesApiController BuildController(Atlas_WebContext context, string root)
    {
        var service = new ReportImageApiService(
            context,
            new MemoryCache(new MemoryCacheOptions()),
            root
        );
        return new ReportImagesApiController(service)
        {
            ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            },
        };
    }

    private static string CreateRoot()
    {
        var root = Path.Combine(Path.GetTempPath(), "atlas-images-" + System.Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path.Combine(root, "wwwroot", "img"));
        return root;
    }

    private static string PlaceholderPath(string root)
    {
        return Path.Combine(root, "wwwroot", "img", "report_placeholder.png");
    }

    [Theory]
    [InlineData("20x20", 40, 20)]
    [InlineData("30x_", 30, 15)]
    [InlineData("500x500", 80, 40)]
    [InlineData("nonsense", 80, 40)]
    [InlineData(null, 80, 40)]
    public void Build_OnlyShrinksToTheRequestedSize(string size, int width, int height)
    {
        using var image = SKBitmap.Decode(ReportThumbnail.Build(Png(80, 40), size, "jpeg"));

        Assert.Equal(width, image.Width);
        Assert.Equal(height, image.Height);
    }

    [Fact]
    public void Build_EncodesWebpWithTransparency()
    {
        var bytes = ReportThumbnail.Build(Png(80, 40), "20x20", "webp");

        using var codec = SKCodec.Create(new MemoryStream(bytes));
        Assert.Equal(SKEncodedImageFormat.Webp, codec.EncodedFormat);
        using var image = SKBitmap.Decode(bytes);
        Assert.Equal(0, image.GetPixel(5, 5).Alpha);
    }

    [Fact]
    public void Build_PutsTransparentImagesOnWhiteForJpeg()
    {
        using var image = SKBitmap.Decode(ReportThumbnail.Build(Png(80, 40), "20x20", "jpeg"));

        Assert.True(image.GetPixel(5, 5).Red > 250);
    }

    [Fact]
    public void Build_ReadsGifUploads()
    {
        var gif = System.Convert.FromBase64String(
            "R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7"
        );

        using var image = SKBitmap.Decode(ReportThumbnail.Build(gif, "20x20", "jpeg"));

        Assert.Equal(1, image.Width);
    }

    [Fact]
    public void Build_RejectsDataThatIsNotAnImage()
    {
        Assert.Throws<InvalidDataException>(() =>
            ReportThumbnail.Build(new byte[] { 1, 2, 3 }, "20x20", "jpeg")
        );
    }

    private static byte[] Png(int width, int height)
    {
        using var image = new SKBitmap(width, height);
        image.Erase(SKColors.Transparent);
        using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
        return encoded.ToArray();
    }
}
