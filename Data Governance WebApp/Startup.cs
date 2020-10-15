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

using Data_Governance_WebApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using WebMarkupMin.AspNet.Common.Compressors;
using WebMarkupMin.AspNetCore2;
using WebMarkupMin.AspNet.Brotli;
using System.IO.Compression;

namespace Data_Governance_WebApp
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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AddPageRoute("/Index/Index", "");
                options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
            });
            // https://docs.microsoft.com/en-us/ef/core/get-started/aspnetcore/existing-db
            var connection = Configuration.GetConnectionString("DataGovernanceDatabase");

            // for linq querys
            services.AddDbContext<Data_GovernanceContext>(options => options.UseSqlServer(connection));

            // for raw sql connection
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.MimeTypes = new[] { "text/plain","text/html","application/xml","text/xml","application/json","text/json", "font/woff2", "application/json; charset = UTF - 8", "text/css", "text/js", "application/css", "application/javascript" };
            });

            services.AddMemoryCache();

            // disabled in place of webmarkupmin.
            //services.AddResponseCaching();

            services.AddWebOptimizer(pipeline =>
            {
                pipeline.AddCssBundle("/css/main.min.css", "lib/fonts/impmin/stylesheet.css",
                                                          "lib/fonts/cheltmin/stylesheet.css",
                                                          "lib/fonts/sfmin/stylesheet.css",
                                                          "css/color.css",
                                                          "css/site.css",
                                                          "css/security.css",
                                                          "css/input.css",
                                                          "css/favorite.css",
                                                          "css/page-controls.css",
                                                          "css/page.css",
                                                          "css/ajax-content.css",
                                                          "css/report.css",
                                                          "css/term.css",
                                                          "css/project.css",
                                                          "css/video.css",
                                                          "css/search.css",
                                                            "css/essential.css",
                                                            "css/utility/carousel.css",
                                                            "css/utility/collapse.css",
                                                            "css/utility/modal.css",
                                                            "css/utility/tabs.css",
                                                            "css/nav.css",
                                                            "css/utility/page_nav.css");
                pipeline.AddCssBundle("/css/differed.min.css","lib/fonts/fontawesome/css/mine.css",
                                                            "css/utility/tooltip.css",
                                                            "css/utility/drag.css",
                                                            "css/utility/progressbar.css",
                                                            "lib/fonts/fasmin/css/fas.css",
                                                            "lib/fonts/farmin/css/far.css",
                                                             "css/utility/chart.css",
                                                          "css/utility/table.css",
                                                          "css/milestone.css",
                                                          "css/checkbox.css",
                                                        "css/comments.css",
                                                          "css/dropdown.css",
                                                           "css/markdown.css",
                                                           "css/messagebox.css",
                                                           "css/toggle.css",
                                                           "lib/scrollbars/simple-scrollbar.css",
                                                           "lib/highlight/rainbow.css");
                pipeline.AddCssBundle("/css/mail.min.css", "css/mail/mail.css",
                                                          "css/mail/mail-folders.css",
                                                          "css/mail/mail-menu.css",
                                                          "css/mail/mail-msgPreview.css",
                                                          "css/mail/mail-notification.css",
                                                          "css/mail/mail-reader.css",
                                                          "css/mail/mail-newMessage.css");
                pipeline.AddCssBundle("/css/editor.min.css", "lib/codemirror/mycss.css",
                                                            "css/live-editor.css");

                pipeline.AddJavaScriptBundle("/js/main.min.js", "js/utility/polyfill.js",
                                                                  "js/essential.js",
                                                                  "js/nav.js",
                                                                  "js/search.js",
                                                                  "lib/scrollbars/simple-scrollbar.min.js");

                pipeline.AddJavaScriptBundle("/js/utility.min.js","js/utility/progressbar.js",
                                                                "js/utility/collapse.js",
                                                                "js/utility/carousel.js",
                                                                "js/utility/modal.js",
                                                                "js/utility/table.js",
                                                                "js/utility/lazyload.js",
                                                                "js/utility/tabs.js",
                                                                "js/utility/drag.js",
                                                                "js/utility/crumbs.js",
                                                                "js/utility/charts.js");

                pipeline.AddJavaScriptBundle("/js/extra.min.js","js/input.js",
                                                                "js/analytics.js",
                                                                "js/page.js",
                                                                "js/comments.js",
                                                                "js/dropdown.js",
                                                                "js/security.js",
                                                                "js/hyperspace.js",
                                                                "js/ajax.js",
                                                                "js/ajax-content.js",
                                                                "js/favorites.js",
                                                                "js/video.js",
                                                                "js/messagebox.js",
                                                                "js/milestone-checklist.js",
                                                                "js/mail.js");

                pipeline.AddJavaScriptBundle("/js/profile.min.js", "js/reportProfile.js");

                pipeline.AddJavaScriptBundle("/js/code.min.js", "lib/highlight/highlight.pack.js");

                pipeline.AddJavaScriptBundle("/js/flowchart.min.js", "lib/flowchart/raphael.min.js", "lib/flowchart/flowchart.min.js", "lib/flowchart/custom.js");
                pipeline.AddJavaScriptBundle("/js/reporteditor.min.js", "js/live-editor.js");
                pipeline.AddJavaScriptBundle("/js/codemirror.min.js", "lib/codemirror/codemirror.js",
                                                                    "lib/codemirror/autorefresh.js",
                                                                    "lib/codemirror/overlay.js",
                                                                    "lib/codemirror/markdown.js",
                                                                    "lib/codemirror/gfm.js",
                                                                    "lib/codemirror/python.js",
                                                                    "lib/codemirror/r.js",
                                                                    "lib/codemirror/shell.js",
                                                                    "lib/codemirror/sql.js"
                                                                    );
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
                            new BrotliCompressorFactory(new BrotliCompressionSettings
                            {
                                Level = 1
                            }),
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
                
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            app.UseResponseCompression();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithRedirects("/Error?id={0}");
                //app.UseExceptionHandler("/Error");
                // enforce https https://aka.ms/aspnetcore-hsts.
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


            if (env.IsDevelopment())
            {
                app.UseMiddleware<StackifyMiddleware.RequestTracerMiddleware>();
                app.UseBrowserLink();
            }

            app.UseMvc();
        }
    }
}
