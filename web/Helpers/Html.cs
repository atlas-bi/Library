using Atlas_Web.Models;
using Markdig;
using Microsoft.AspNetCore.Mvc.Razor;
using WebOptimizer;
using Slugify;
using Ganss.Xss;
using WebMarkupMin.Core;

namespace Atlas_Web.Helpers
{
    public static class HtmlHelpers
    {
        //https://github.com/ligershark/WebOptimizer/issues/87
        public static string AddBundleVersionToPath(this IRazorPage page, string path)
        {
            var context = page.ViewContext.HttpContext;
            var assetPipeline = context.RequestServices.GetService<WebOptimizer.IAssetPipeline>();

            if (assetPipeline.TryGetAssetFromRoute(path, out IAsset asset))
            {
                return string.Concat(
                    path,
                    "?v=",
                    asset.GenerateCacheKey(context, new WebOptimizerOptions())
                );
            }
            else
            {
                return path;
            }
        }

        public static string Slug(string stuff)
        {
            SlugHelper helper = new SlugHelper();
            return helper.GenerateSlug(stuff);
        }

        public static string SiteMessage(HttpContext _httpContext, Atlas_WebContext _context)
        {
            string text = "";
            // site wide message
            GlobalSiteSetting msg = _context.GlobalSiteSettings
                .Where(x => x.Name == "msg" && x.Value == null)
                .FirstOrDefault();

            if (msg != null && msg.Description != null && msg.Description != "")
            {
                text += msg.Description;
            }

            if (_httpContext.Request.Query["msg"].ToString() != "")
            {
                // url message
                msg = _context.GlobalSiteSettings
                    .Where(
                        x =>
                            x.Name == "msg"
                            && x.Value == _httpContext.Request.Query["msg"].ToString()
                            && x.Value != null
                    )
                    .FirstOrDefault();

                if (msg != null && msg.Description != null && msg.Description != "")
                {
                    text += msg.Description;
                }
            }
            return text;
        }

        public static string MarkdownToHtml(string text, IConfiguration _config)
        {
            if (text is null)
            {
                return "";
            }
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseAutoLinks()
                .UseSoftlineBreakAsHardlineBreak()
                .UseEmojiAndSmiley()
                .UseSmartyPants()
                .UseDiagrams()
                .UseFootnotes()
                .Build();

            var sanitizer = new HtmlSanitizer();
            return sanitizer.Sanitize(
                Markdown.ToHtml(text, pipeline),
                (
                    string.IsNullOrEmpty(_config["AppSettings:base_url"])
                        ? ""
                        : _config["AppSettings:base_url"]
                )
            );
        }

        public static string MinifyHtml(string text)
        {
            var htmlMinifier = new HtmlMinifier();
            MarkupMinificationResult result = htmlMinifier.Minify(text);
            if (result.Errors.Count == 0)
            {
                return result.MinifiedContent;
            }
            return text;
        }

        public static string MarkdownToText(string text)
        {
            if (text is null)
            {
                return "";
            }
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseAutoLinks()
                .UseSoftlineBreakAsHardlineBreak()
                .UseEmojiAndSmiley()
                .UseSmartyPants()
                .UseDiagrams()
                .UseFootnotes()
                .Build();
            return Markdown.ToPlainText(text, pipeline);
        }
    }
}
