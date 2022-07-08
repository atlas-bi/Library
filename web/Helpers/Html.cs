using System;
using Atlas_Web.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Markdig;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using WebOptimizer;
using Slugify;

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

        public static string MarkdownToHtml(string text)
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
            return Markdown.ToHtml(text, pipeline);
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
