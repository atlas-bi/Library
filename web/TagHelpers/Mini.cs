using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using NUglify;
using WebOptimizer;
using WebOptimizer.Taghelpers;

namespace Atlas_Web.TagHelpers
{
    [HtmlTargetElement("style", Attributes = "mini")]
    [HtmlTargetElement("script", Attributes = "mini")]
    public class MiniOnDemandTagHelper : BaseTagHelper
    {
        public MiniOnDemandTagHelper(
            IWebHostEnvironment env,
            IMemoryCache cache,
            IAssetPipeline pipeline,
            IOptionsMonitor<WebOptimizerOptions> options
        ) : base(env, cache, pipeline, options) { }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var input = (await output.GetChildContentAsync()).GetContent();

            UglifyResult result;
            result = context.TagName.Equals("script", StringComparison.InvariantCultureIgnoreCase)
              ? Uglify.Js(input)
              : Uglify.Css(input);

            var minifyAttribute = output.Attributes.First(
                x => x.Name.Equals("mini", StringComparison.InvariantCultureIgnoreCase)
            );
            output.Attributes.Remove(minifyAttribute);

            output.Content.SetHtmlContent(result.Code);
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}
