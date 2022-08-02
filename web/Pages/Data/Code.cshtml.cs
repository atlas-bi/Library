using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebOptimizer;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Options;
using System.Text;

namespace Atlas_Web.Pages.Data
{
    public class CodeModel : PageModel
    {
        private readonly IAssetPipeline _pipeline;
        private readonly IAssetBuilder _assetBuilder;
        private readonly IOptionsSnapshot<WebOptimizerOptions> _options;

        public CodeModel(
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
            string hash = asset.GenerateCacheKey(HttpContext, new WebOptimizerOptions());
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
