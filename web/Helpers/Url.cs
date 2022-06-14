using Atlas_Web.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Diagnostics.Contracts;
using Microsoft.Extensions.Caching.Memory;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;

namespace Atlas_Web.Helpers
{
    public static class UrlHelpers
    {
        private static string GenerateHash(string content)
        {
            using var algo = SHA1.Create();
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
            byte[] hash = algo.ComputeHash(buffer);
            return WebEncoders.Base64UrlEncode(hash);
        }

        [Pure]
        public static string FontHash(PathString font, IMemoryCache cache)
        {
            // this function exists because ligershark.assets as a v=? hash to font files in
            // the css build, but doesn't add them to the preload tags.
            // they never get added to the cache, or we could pull the url there,
            // the code is basically copy/paste from ligershark
            // we add it to cache to speed up next time

            if (cache.Get(font.Value) != null)
            {
                return cache.Get(font.Value).ToString();
            }

            var info = new FileInfo(Path.Combine("wwwroot/", font.Value.TrimStart('/')));
            string hash = GenerateHash(info.LastWriteTime.Ticks.ToString());
            string withHash = font.Value + $"?v={hash}";

            cache.Set(font.Value, withHash);
            return withHash;
        }

        [Pure]
        public static string SetParameters(
            HttpContext helper,
            IDictionary<string, string> parameters
        )
        {
            var qs = QueryHelpers.ParseQuery(helper.Request.QueryString.Value ?? "");
            // if we are changing type, we should remove all other filters.
            if (parameters.ContainsKey("type"))
            {
#pragma warning disable S3267
                foreach (var p in qs)
                {
#pragma warning restore S3267
                    if (
                        p.Key != "type"
                        && p.Key != "Query"
                        && p.Key != "advanced"
                        && !(parameters["type"] == "reports" && p.Key == "report_type_text")
                    )
                    {
                        qs.Remove(p.Key);
                    }
                }
                foreach (var p in parameters)
                {
                    // if we need an "uncheck" url, the pop the key.
                    if (qs.ContainsKey(p.Key) && qs[p.Key] == p.Value)
                    {
                        qs.Remove(p.Key);
                    }
                    else
                    {
                        qs[p.Key] = p.Value;
                    }
                }
            }
            else
            {
                foreach (var p in parameters)
                {
                    // if we need an "uncheck" url, the pop the key.
                    if (qs.ContainsKey(p.Key) && qs[p.Key] == p.Value)
                    {
                        qs.Remove(p.Key);
                    }
                    else
                    {
                        qs[p.Key] = p.Value;
                        if (p.Key == "report_type_text")
                        {
                            qs["type"] = "reports";
                        }
                    }
                }
            }
            return helper.Request.Path + QueryString.Create(qs);
        }

        [Pure]
        public static string SetSearchPageUrl(HttpContext helper, int pageIndex)
        {
            return SetParameters(
                helper,
                new Dictionary<string, string>
                {
                    { nameof(SolrAtlasParameters.PageIndex), pageIndex.ToString() }
                }
            );
        }

        [Pure]
        public static string SetSearchFacetUrl(HttpContext helper, string facet, string value)
        {
            return SetParameters(helper, new Dictionary<string, string> { { facet, value } });
        }

        [Pure]
        public static bool SetSearchFacetChecked(
            SolrAtlasParameters parameter,
            string facet,
            string value
        ) => parameter.Filters.ContainsKey(facet) && parameter.Filters[facet].Contains(value);

        [Pure]
        public static bool HasFacet(SolrAtlasParameters parameter, string facet) =>
            parameter.Filters.ContainsKey(facet);
    }
}
