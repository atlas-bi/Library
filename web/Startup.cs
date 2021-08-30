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
using System.Collections.Generic;
using System.IO.Compression;
using WebMarkupMin.AspNet.Common.Compressors;
using WebMarkupMin.AspNetCore5;

namespace Atlas_Web
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

            services.AddRazorPages().AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/Index/Index", "");
                options.Conventions.AddPageRoute("/Index/About", "about_analytics");
                options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
            });


            var connection = Configuration.GetConnectionString("AtlasDatabase");

            // for linq querys
            services.AddDbContext<Atlas_WebContext>(options => options.UseSqlServer(connection));

            // for raw sql connection
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.MimeTypes = new[] { "text/plain", "text/html", "application/xml", "text/xml", "application/json", "text/json", "font/woff2", "application/json; charset = UTF - 8", "text/css", "text/js", "application/css", "application/javascript" };
            });

            services.AddMemoryCache();

            services.AddWebOptimizer(pipeline =>
            {
                pipeline.AddCssBundle("/css/main.min.css", "css/essential.css",
                                                            "lib/fonts/impmin/stylesheet.css",
                                                            "lib/fonts/cheltmin/stylesheet.css",
                                                            "lib/fonts/sfmin/stylesheet.css",
                                                            "lib/fonts/marckscript/stylesheet.css",
                                                            "css/nav.css",
                                                            "css/favorite.css",
                                                            "css/page.css",
                                                            "css/video.css",
                                                            "css/search.css",
                                                            "css/utility/collapse.css",
                                                            "css/utility/modal.css",
                                                            "css/utility/tabs.css");

                pipeline.AddCssBundle("/css/differed.min.css", "css/nav-effects.css",
                                                            "css/page-effects.css",
                                                            "css/favorite-effects.css",
                                                            "lib/fonts/fontawesome/css/mine.css",
                                                            "css/utility/tooltip.css",
                                                            "css/utility/drag.css",
                                                            "css/utility/progressbar.css",
                                                            "lib/fonts/fasmin/css/fas.css",
                                                            "lib/fonts/farmin/css/far.css",
                                                            "css/utility/chart.css",
                                                            "css/utility/table.css",
                                                            "css/milestone.css",
                                                            "css/input.css",
                                                            "css/comments.css",
                                                            "css/dropdown.css",
                                                            "css/markdown.css",
                                                            "css/messagebox.css",
                                                            "lib/scrollbars/simple-scrollbar.css",
                                                            "lib/highlight/rainbow.css",
                                                            "css/mail/mail-newMessage.css",
                                                             "css/query.css");

                pipeline.AddCssBundle("/css/reports.min.css", "css/reports.css", "css/utility/carousel.css");
                pipeline.AddCssBundle("/css/access.min.css", "css/access.css");
                pipeline.AddCssBundle("/css/tasks.min.css", "css/tasks.css");
                pipeline.AddCssBundle("/css/users.min.css", "css/users.css");
                pipeline.AddCssBundle("/css/error.min.css", "css/error.css");

                pipeline.AddCssBundle("/css/mail.min.css", "css/mail/mail.css",
                                                           "css/mail/mail-folders.css",
                                                           "css/mail/mail-menu.css",
                                                           "css/mail/mail-msgPreview.css",
                                                           "css/mail/mail-notification.css",
                                                           "css/mail/mail-reader.css");

                pipeline.AddCssBundle("/css/editor.min.css", "lib/codemirror/mycss.css",
                                                             "css/live-editor.css");

                /************ auto prefix css ************/
                pipeline.MinifyCssFiles().AutoPrefixCss();

                /************   javascript   *************/

                // for ie11
                pipeline.AddJavaScriptBundle("/js/polyfill.min.js", "js/utility/polyfill.js");

                pipeline.AddJavaScriptBundle("/js/realtime.min.js", "js/realtime.js");

                // required for page load     
                pipeline.AddJavaScriptBundle("/js/main.min.js", "js/essential.js",
                                                                "lib/scrollbars/simple-scrollbar.js");
                // required for search
                pipeline.AddJavaScriptBundle("/js/search.min.js", "js/search.js", "js/utility/progressbar.js");

                // used on all pages, but not for load
                pipeline.AddJavaScriptBundle("/js/utility.min.js", "js/utility/modal.js",
                                                                  "js/utility/lazyload.js",
                                                                  "js/utility/crumbs.js",
                                                                  "js/page.js",
                                                                  "js/hyperspace.js",
                                                                  "js/ajax-content.js",
                                                                  "js/favorites.js",
                                                                  "js/video.js",
                                                                  "js/messagebox.js",
                                                                  "js/mail.js",
                                                                  "js/analytics.js");

                pipeline.AddJavaScriptBundle("/js/tabs.min.js", "js/utility/tabs.js");
                pipeline.AddJavaScriptBundle("/js/collapse.min.js", "js/utility/collapse.js");
                pipeline.AddJavaScriptBundle("/js/carousel.min.js", "js/utility/carousel.js");
                pipeline.AddJavaScriptBundle("/js/table.min.js", "js/utility/table.js");
                pipeline.AddJavaScriptBundle("/js/drag.min.js", "js/utility/drag.js");
                pipeline.AddJavaScriptBundle("/js/charts.min.js", "js/utility/charts.js");
                pipeline.AddJavaScriptBundle("/js/input.min.js", "js/input.js");
                pipeline.AddJavaScriptBundle("/js/comments.min.js", "js/comments.js");
                pipeline.AddJavaScriptBundle("/js/dropdown.min.js", "js/dropdown.js");

                pipeline.AddJavaScriptBundle("/js/milestone-checklist.min.js", "js/milestone-checklist.js");

                pipeline.AddJavaScriptBundle("/js/access.min.js", "js/access.js");

                pipeline.AddJavaScriptBundle("/js/profile.min.js", "js/profile.js");

                pipeline.AddJavaScriptBundle("/js/code.min.js", "lib/highlight/highlight.js");

                pipeline.AddJavaScriptBundle("/js/flowchart.min.js", "lib/flowchart/raphael.min.js", "lib/flowchart/flowchart.min.js", "lib/flowchart/custom.js");

                pipeline.AddJavaScriptBundle("/js/editor.min.js", "lib/codemirror/codemirror.js",
                                                                  "lib/codemirror/autorefresh.js",
                                                                  "lib/codemirror/overlay.js",
                                                                  "lib/codemirror/markdown.js",
                                                                  "lib/codemirror/gfm.js",
                                                                  "lib/codemirror/python.js",
                                                                  "lib/codemirror/r.js",
                                                                  "lib/codemirror/shell.js",
                                                                  "lib/codemirror/sql.js",
                                                                  "js/editor.js",
                                                                  "js/utility/checkbox.js"
                                                                  );
                pipeline.AddJavaScriptBundle("/js/report-editor.min.js", "js/reportEditor.js");
                pipeline.AddJavaScriptBundle("/js/collection-editor.min.js", "js/collectionEditor.js");
            });

            services.AddWebMarkupMin(
                options =>
                {
                    options.AllowMinificationInDevelopmentEnvironment = true;
                    options.AllowCompressionInDevelopmentEnvironment = true;
                })
                .AddHtmlMinification(
                    options =>
                    {
                        options.MinificationSettings.RemoveRedundantAttributes = true;
                        options.MinificationSettings.RemoveHttpProtocolFromAttributes = true;
                        options.MinificationSettings.RemoveHttpsProtocolFromAttributes = true;
                    })
                .AddHttpCompression(options =>
                    {
                        options.CompressorFactories = new List<ICompressorFactory>
                        {
                            new DeflateCompressorFactory(new DeflateCompressionSettings
                            {
                                Level = CompressionLevel.Fastest
                            }),
                            new GZipCompressorFactory(new GZipCompressionSettings
                            {
                                Level = CompressionLevel.Fastest
                            })
                        };
                    });

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseMiddleware<StackifyMiddleware.RequestTracerMiddleware>();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("/Error?id={0}");
                app.UseHsts();
            }


            app.UseWebMarkupMin();
            app.UseWebOptimizer();
            app.UseHttpsRedirection();

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
                });

            //app.UseCookiePolicy();

            app.UseRouting();
            // app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
