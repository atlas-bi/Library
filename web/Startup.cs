using Atlas_Web.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using WebMarkupMin.AspNet.Common.Compressors;
using WebMarkupMin.AspNetCore5;
using SolrNet;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using Atlas_Web.Middleware;
using Atlas_Web.Services;
using Atlas_Web.Helpers;
using Newtonsoft.Json.Linq;
using Hangfire;
using Hangfire.InMemory;

namespace Atlas_Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostingEnvironment = hostingEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(
                configuration =>
                    configuration
                        //.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseInMemoryStorage()
                        .WithJobExpirationTimeout(TimeSpan.FromHours(1))
            );

            // Add the processing server as IHostedService
            services.AddHangfireServer();

            services.Configure<CookiePolicyOptions>(
                options =>
                {
                    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
                }
            );
            services.AddResponseCaching();

            services
                .AddRazorPages()
                .AddRazorPagesOptions(
                    options =>
                    {
                        options.Conventions.AddPageRoute("/Index/Index", "");
                        options.Conventions.AddPageRoute("/Index/About", "about_analytics");
                        options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
                    }
                );

            services.AddSolrNet<SolrAtlas>(Configuration["solr:atlas_address"]);
            services.AddSolrNet<SolrAtlasLookups>(Configuration["solr:atlas_lookups_address"]);

            var connection = Configuration.GetConnectionString("AtlasDatabase");

            // for linq queries
            services.AddDbContext<Atlas_WebContext>(
                options =>
                    options.UseSqlServer(
                        connection,
                        o =>
                            o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                                .CommandTimeout(60000)
                    )
            );

            // for raw sql connection
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddResponseCompression(
                options =>
                {
                    options.EnableForHttps = true;
                    options.MimeTypes = new[]
                    {
                        "text/plain",
                        "text/html",
                        "application/xml",
                        "text/xml",
                        "application/json",
                        "text/json",
                        "font/woff2",
                        "application/json; charset = UTF - 8",
                        "text/css",
                        "text/js",
                        "application/css",
                        "application/javascript"
                    };
                }
            );

            services.AddMemoryCache();

            var cssSettings = new CssBundlingSettings { Minify = true, FingerprintUrls = true, };
            var codeSettings = new CodeBundlingSettings { Minify = true, };

            services.AddWebOptimizer(
                HostingEnvironment,
                cssSettings,
                codeSettings,
                pipeline =>
                {
                    pipeline.AddCssBundle("/css/site.min.css", "css/site.min.css");
                    pipeline.AddCssBundle("/css.email.min.css", "email.min.css");

                    /************   javascript   *************/
                    pipeline.AddJavaScriptBundle("/js/polyfill.min.js", "js/polyfill.min.js");
                    pipeline.AddJavaScriptBundle(
                        "/js/purify.min.js",
                        "lib/dompurify/purify.min.js"
                    );
                    pipeline.AddJavaScriptBundle("/js/shared.min.js", "js/shared.min.js");
                    pipeline.AddJavaScriptBundle("/js/realtime.min.js", "js/realtime.js");

                    // required for page load
                    pipeline.AddJavaScriptBundle(
                        "/js/main.min.js",
                        "js/essential.js",
                        "lib/scrollbars/simple-scrollbar.js"
                    );
                    // required for search
                    pipeline.AddJavaScriptBundle("/js/search.min.js", "js/search.min.js");

                    // used on all pages, but not for load
                    pipeline.AddJavaScriptBundle("/js/utility.min.js", "js/utility.min.js");

                    pipeline.AddJavaScriptBundle("/js/settings.min.js", "js/settings.min.js");

                    pipeline.AddJavaScriptBundle("/js/profile.min.js", "js/profile.min.js");

                    pipeline.AddJavaScriptBundle("/js/code.min.js", "js/highlight.min.js");

                    pipeline.AddJavaScriptBundle("/js/analytics.min.js", "js/analytics.min.js");
                    pipeline.AddJavaScriptBundle("/js/alive.min.js", "js/alive.min.js");

                    pipeline.AddJavaScriptBundle(
                        "/js/flowchart.min.js",
                        "lib/flowchart/raphael.min.js",
                        "lib/flowchart/flowchart.min.js",
                        "lib/flowchart/custom.js"
                    );

                    pipeline.AddJavaScriptBundle(
                        "/js/editor.min.js",
                        "lib/codemirror/codemirror.js",
                        "lib/codemirror/autorefresh.js",
                        "lib/codemirror/overlay.js",
                        "lib/codemirror/markdown.js",
                        "lib/codemirror/gfm.js",
                        "lib/codemirror/python.js",
                        "lib/codemirror/r.js",
                        "lib/codemirror/shell.js",
                        "lib/codemirror/sql.js",
                        "lib/codemirror/spellcheck.js",
                        "/js/editor.min.js"
                    );
                }
            );

            services
                .AddWebMarkupMin(
                    options =>
                    {
                        options.AllowMinificationInDevelopmentEnvironment = true;
                        options.AllowCompressionInDevelopmentEnvironment = true;
                    }
                )
                .AddHtmlMinification(
                    options =>
                    {
                        options.MinificationSettings.RemoveRedundantAttributes = true;
                        options.MinificationSettings.RemoveHttpProtocolFromAttributes = true;
                        options.MinificationSettings.RemoveHttpsProtocolFromAttributes = true;
                    }
                )
                .AddHttpCompression(
                    options =>
                    {
                        options.CompressorFactories = new List<ICompressorFactory>
                        {
                            new DeflateCompressorFactory(
                                new DeflateCompressionSettings { Level = CompressionLevel.Fastest }
                            ),
                            new GZipCompressorFactory(
                                new GZipCompressionSettings { Level = CompressionLevel.Fastest }
                            )
                        };
                    }
                );

            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IRazorPartialToStringRenderer, RazorPartialToStringRenderer>();

            services.Configure<IISServerOptions>(
                options =>
                {
                    options.AllowSynchronousIO = true;
                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            Atlas_WebContext context,
            IMemoryCache cache
        )
        {
            app.UseResponseCompression();

            if (!env.IsDevelopment())
            {
                app.UseHsts();

                app.UseHttpsRedirection();
                app.UseStatusCodePagesWithReExecute("/Error", "?id={0}");
                app.UseExceptionHandler("/Error");
            }
            else
            {
                app.UseHangfireDashboard();
                app.UseDeveloperExceptionPage();
            }

            app.UseWebMarkupMin();
            app.UseWebOptimizer();

            app.UseStaticFiles(
                new StaticFileOptions
                {
                    OnPrepareResponse = ctx =>
                    {
                        var headers = ctx.Context.Response.GetTypedHeaders();
                        headers.CacheControl = new CacheControlHeaderValue
                        {
                            Public = true,
                            MaxAge = TimeSpan.FromDays(365)
                        };
                    }
                }
            );

            app.UseETagger();
            app.UseRouting();

            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapRazorPages();
                }
            );

            app.UseResponseCaching();
            app.Use(
                async (context, next) =>
                {
                    context.Response.GetTypedHeaders().CacheControl =
                        new Microsoft.Net.Http.Headers.CacheControlHeaderValue
                        {
                            Public = true,
                            MaxAge = TimeSpan.FromMinutes(20)
                        };
                    await next();
                }
            );

            // load override css
            var css = context.GlobalSiteSettings
                .Where(x => x.Name == "global_css")
                .Select(x => x.Value)
                .FirstOrDefault();
            if (css != null)
            {
                cache.Set("global_css", css);
            }

            // set logo
            if (System.IO.File.Exists(Configuration["logo"]))
            {
                try
                {
                    byte[] imageArray = System.IO.File.ReadAllBytes(Configuration["logo"]);
                    string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                    cache.Set("logo", "data:image/png;base64," + base64ImageRepresentation);
                    cache.Set("logo_path", Configuration["logo"]);
                }
                catch
                {
                    cache.Set("logo", "/img/atlas-logo-smooth.png");
                    cache.Set("logo_path", "wwwroot/img/atlas-logo-smooth.png");
                }
            }
            else
            {
                cache.Set("logo", "/img/atlas-logo-smooth.png");
                cache.Set("logo_path", "wwwroot/img/atlas-logo-smooth.png");
            }
            // set version
            try
            {
                var d = "";

                if (File.Exists("version"))
                {
                    d = File.ReadAllText("version");
                }

                if (!string.IsNullOrEmpty(d))
                {
                    cache.Set("version", d);
                }
            }
            catch
            {
                // not set
            }


        }
    }
}
