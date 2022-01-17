/*
    Atlas of Information Management business intelligence library and documentation database.
    Copyright (C) 2020  Riverside Healthcare, Kankakee, IL

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebOptimizer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Options;
using System.Text;

namespace Atlas_Web.Pages.Data
{
    public class CodeModel : PageModel
    {
        private IAssetPipeline _pipeline;
        private IAssetBuilder _assetBuilder;
        private IOptionsSnapshot<WebOptimizerOptions> _options;

        public CodeModel(
            IMemoryCache cache,
            IAssetPipeline pipeline,
            IAssetBuilder assetBuilder,
            IOptionsSnapshot<WebOptimizerOptions> options
        )
        {
            _pipeline = pipeline;
            _assetBuilder = assetBuilder;
            _options = options;
        }

        protected string GenerateHash(IAsset asset)
        {
            string hash = asset.GenerateCacheKey(HttpContext);
            return $"{asset.Route}?v={hash}";
        }

        public async Task<ActionResult> OnGetContentAsync(string id)
        {
            _pipeline.TryGetAssetFromRoute(id, out IAsset asset);

            var src = GenerateHash(asset);

            IAssetResponse response = await _assetBuilder.BuildAsync(
                asset,
                HttpContext,
                _options.Value
            );
            IAssetResponse cachedResponse = response;
            string cacheKey = response.CacheKey;
            HttpContext.Response.ContentType = asset.ContentType;

            foreach (string name in cachedResponse.Headers.Keys)
            {
                HttpContext.Response.Headers[name] = cachedResponse.Headers[name];
            }

            HttpContext.Response.Headers[HeaderNames.ETag] = src.Split("v=")[1];
            return Content(
                Encoding.UTF8.GetString(cachedResponse.Body, 0, cachedResponse.Body.Length)
            );
        }

        public ActionResult OnGet(string id)
        {
            _pipeline.TryGetAssetFromRoute(id, out IAsset asset);

            return Content(GenerateHash(asset));
        }
    }
}
