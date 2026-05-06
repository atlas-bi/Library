using System.Data.SqlClient;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Atlas_Web;
using Atlas_Web.Authentication;
using Atlas_Web.Authorization;
using Atlas_Web.Middleware;
using Atlas_Web.Models;
using Atlas_Web.Services;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using ITfoxtec.Identity.Saml2.Util;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using SolrNet;
using WebMarkupMin.AspNet.Common.Compressors;
using WebMarkupMin.AspNetCore5;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.cust.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile(
    $"appsettings.cust.{builder.Environment.EnvironmentName}.json",
    optional: true,
    reloadOnChange: true
);
builder.Configuration.AddEnvironmentVariables();

builder.WebHost.CaptureStartupErrors(true);

builder.Services.AddHangfire(configuration =>
    configuration
        //.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseInMemoryStorage()
        .WithJobExpirationTimeout(TimeSpan.FromHours(1))
);

builder.Services.AddHangfireServer();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
});
builder.Services.AddResponseCaching();

ProgramConfiguration.ConfigureCors(builder);

// for linq queries - conditionally register based on environment
if (!builder.Environment.IsEnvironment("Test"))
{
    builder.Services.AddDbContext<Atlas_WebContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("AtlasDatabase"),
            o =>
                o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery).CommandTimeout(60000)
        )
    );
}

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddResponseCompression(options =>
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
        "application/javascript",
    };
});

builder.Services.AddMemoryCache();

builder
    .Services.AddDataProtection()
    .UseCryptographicAlgorithms(
        new AuthenticatedEncryptorConfiguration()
        {
            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
            ValidationAlgorithm = ValidationAlgorithm.HMACSHA256,
        }
    );

var cssSettings = new CssBundlingSettings { Minify = true, FingerprintUrls = true };
var codeSettings = new CodeBundlingSettings { Minify = true };

builder.Services.AddWebOptimizer(
    builder.Environment,
    cssSettings,
    codeSettings,
    pipeline =>
    {
        pipeline.AddCssBundle("/css/site.min.css", "css/site.min.css");
        pipeline.AddCssBundle("/css.email.min.css", "email.min.css");

        /************   javascript   *************/
        pipeline.AddJavaScriptBundle("/js/polyfill.min.js", "js/polyfill.min.js");
        pipeline.AddJavaScriptBundle("/js/purify.min.js", "lib/dompurify/purify.min.js");
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

        pipeline.AddJavaScriptBundle("/js/hyperspace.min.js", "js/hyperspace.min.js");

        pipeline.AddJavaScriptBundle("/js/settings.min.js", "js/settings.min.js");

        pipeline.AddJavaScriptBundle("/js/profile.min.js", "js/profile.min.js");

        pipeline.AddJavaScriptBundle("/js/code.min.js", "js/highlight.min.js");
        pipeline.AddJavaScriptBundle("/js/user-settings.min.js", "js/user-settings.min.js");

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
            "js/editor.min.js"
        );
    }
);

builder
    .Services.AddWebMarkupMin(options =>
    {
        options.AllowMinificationInDevelopmentEnvironment = true;
        options.AllowCompressionInDevelopmentEnvironment = true;
    })
    .AddHtmlMinification(options =>
    {
        options.MinificationSettings.RemoveRedundantAttributes = true;
        options.MinificationSettings.RemoveHttpProtocolFromAttributes = true;
        options.MinificationSettings.RemoveHttpsProtocolFromAttributes = true;
    })
    .AddHttpCompression(options =>
    {
        options.CompressorFactories = new List<ICompressorFactory>
        {
            new DeflateCompressorFactory(
                new DeflateCompressionSettings { Level = CompressionLevel.Fastest }
            ),
            new GZipCompressorFactory(
                new GZipCompressionSettings { Level = CompressionLevel.Fastest }
            ),
        };
    });

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<IRazorPartialToStringRenderer, RazorPartialToStringRenderer>();
builder.Services.AddScoped<ICollectionsApiService, CollectionsApiService>();
builder.Services.AddScoped<IReportsApiService, ReportsApiService>();
builder.Services.AddScoped<ISearchApiService, SearchApiService>();
builder.Services.AddHttpContextAccessor();

ProgramConfiguration.ConfigureJwtAuthentication(builder);
if (builder.Configuration.GetSection("Saml2").Exists())
{
    builder.Services.AddHttpClient();
    builder.Services.BindConfig<Saml2Configuration>(
        builder.Configuration,
        "Saml2",
        (serviceProvider, saml2Configuration) =>
        {
            //saml2Configuration.SignAuthnRequest = true;
            saml2Configuration.SigningCertificate = CertificateUtil.Load(
                builder.Environment.MapToPhysicalFilePath(
                    builder.Configuration["Saml2:SigningCertificateFile"]
                ),
                builder.Configuration["Saml2:SigningCertificatePassword"],
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet
            );

            //saml2Configuration.SignatureValidationCertificates.Add(CertificateUtil.Load(AppEnvironment.MapToPhysicalFilePath(Configuration["Saml2:SignatureValidationCertificateFile"])));
            saml2Configuration.AllowedAudienceUris.Add(saml2Configuration.Issuer);

            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            var entityDescriptor = new EntityDescriptor();
            entityDescriptor
                .ReadIdPSsoDescriptorFromUrlAsync(
                    httpClientFactory,
                    new Uri(builder.Configuration["Saml2:IdPMetadata"])
                )
                .GetAwaiter()
                .GetResult();
            if (entityDescriptor.IdPSsoDescriptor != null)
            {
                saml2Configuration.AllowedIssuer = entityDescriptor.EntityId;
                saml2Configuration.SingleSignOnDestination = entityDescriptor
                    .IdPSsoDescriptor.SingleSignOnServices.First()
                    .Location;
                // saml2Configuration.SingleLogoutDestination = entityDescriptor.IdPSsoDescriptor.SingleLogoutServices.First().Location;
                foreach (
                    var signingCertificate in entityDescriptor.IdPSsoDescriptor.SigningCertificates
                )
                {
                    if (signingCertificate.IsValidLocalTime())
                    {
                        saml2Configuration.SignatureValidationCertificates.Add(signingCertificate);
                    }
                }
                if (saml2Configuration.SignatureValidationCertificates.Count <= 0)
                {
                    throw new Exception("The IdP signing certificates has expired.");
                }
                if (entityDescriptor.IdPSsoDescriptor.WantAuthnRequestsSigned.HasValue)
                {
                    saml2Configuration.SignAuthnRequest = entityDescriptor
                        .IdPSsoDescriptor
                        .WantAuthnRequestsSigned
                        .Value;
                }
            }
            else
            {
                throw new Exception("IdPSsoDescriptor not loaded from metadata.");
            }

            return saml2Configuration;
        }
    );

    builder.Services.AddSaml2(slidingExpiration: true);
}

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    options.AddPolicy(
        "ReportRunPolicy",
        policy => policy.Requirements.Add(new PermissionRequirement())
    );
});
builder.Services.AddSingleton<IAuthorizationHandler, ReportRunAuthorizationHandler>();
builder.Services.AddScoped<IClaimsTransformation, CustomClaimsTransformer>();

builder.Services.Configure<IISServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

builder
    .Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AddPageRoute("/Index/Index", "");
        options.Conventions.AddPageRoute("/Index/About", "about_analytics");
        options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
    })
    .AddRazorRuntimeCompilation();
builder.Services.AddControllers();
builder.Services.AddSolrNet<SolrAtlas>(builder.Configuration["solr:atlas_address"]);
builder.Services.AddSolrNet<SolrAtlasLookups>(builder.Configuration["solr:atlas_lookups_address"]);

var app = builder.Build();

app.UseResponseCompression();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
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
                MaxAge = TimeSpan.FromDays(365),
            };
        },
    }
);

app.UseETagger();
app.UseRouting();
app.UseCors("NextJs");
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Use(
    async (context, next) =>
    {
        context.Response.Headers.Add("Content-Security-Policy", "frame-ancestors 'self' *;");
        await next();
    }
);

// Skip database initialization in Test environment to avoid provider conflicts
if (!app.Environment.IsEnvironment("Test"))
{
    using (var scope = app.Services.CreateScope())
    {
        IMemoryCache cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        Atlas_WebContext context = scope.ServiceProvider.GetRequiredService<Atlas_WebContext>();

        if (context.Database.IsRelational())
        {
            try
            {
                context.Database.Migrate();
            }
            catch (SqlException ex) when (ex.Number == 4060)
            {
                var connStr = app.Configuration.GetConnectionString("AtlasDatabase");
                var csb = new SqlConnectionStringBuilder(connStr) { InitialCatalog = "master" };

                using (var conn = new SqlConnection(csb.ConnectionString))
                {
                    conn.Open();
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = "IF DB_ID(N'atlas') IS NULL CREATE DATABASE [atlas];";
                    cmd.ExecuteNonQuery();
                }

                context.Database.Migrate();
            }
        }

        var seedDemoRaw =
            app.Configuration["SEED_DEMO"] ?? Environment.GetEnvironmentVariable("SEED_DEMO");
        var shouldSeedDemo =
            !string.IsNullOrWhiteSpace(seedDemoRaw)
            && (
                string.Equals(seedDemoRaw, "true", StringComparison.OrdinalIgnoreCase)
                || string.Equals(seedDemoRaw, "1", StringComparison.OrdinalIgnoreCase)
                || string.Equals(seedDemoRaw, "yes", StringComparison.OrdinalIgnoreCase)
            );

        if (shouldSeedDemo)
        {
            const string seedMarkerName = "demo_seed_applied";
            var alreadySeeded = context.GlobalSiteSettings.Any(x => x.Name == seedMarkerName);

            if (!alreadySeeded)
            {
                var seedScriptPath = Path.Combine(
                    AppContext.BaseDirectory,
                    "atlas-demo-seed_script.sql"
                );
                if (File.Exists(seedScriptPath))
                {
                    var seedSql = File.ReadAllText(seedScriptPath);
                    var batches = Regex.Split(
                        seedSql,
                        @"^\s*GO\s*$",
                        RegexOptions.Multiline | RegexOptions.IgnoreCase,
                        TimeSpan.FromSeconds(5)
                    );

                    using var connection = new SqlConnection(
                        app.Configuration.GetConnectionString("AtlasDatabase")
                    );
                    connection.Open();

                    using var tx = connection.BeginTransaction();
                    try
                    {
                        foreach (var batch in batches)
                        {
                            var sql = batch?.Trim();
                            if (string.IsNullOrWhiteSpace(sql))
                            {
                                continue;
                            }

                            using var cmd = connection.CreateCommand();
                            cmd.Transaction = tx;
                            cmd.CommandTimeout = 60000;
                            cmd.CommandText = sql;
                            cmd.ExecuteNonQuery();
                        }

                        context.GlobalSiteSettings.Add(
                            new GlobalSiteSetting
                            {
                                Name = seedMarkerName,
                                Description = "",
                                Value = DateTimeOffset.UtcNow.ToString("O"),
                            }
                        );
                        context.SaveChanges();

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        // load override css
        var css = context
            .GlobalSiteSettings.Where(x => x.Name == "global_css")
            .Select(x => x.Value)
            .FirstOrDefault();
        if (css != null)
        {
            cache.Set("global_css", css);
        }

        // set logo
        var logoPath = app.Configuration["logo"];
        if (!string.IsNullOrWhiteSpace(logoPath) && System.IO.File.Exists(logoPath))
        {
            try
            {
                byte[] imageArray = System.IO.File.ReadAllBytes(logoPath);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);
                cache.Set("logo", "data:image/png;base64," + base64ImageRepresentation);
                cache.Set("logo_path", logoPath);
            }
            catch
            {
                // cache.Set("logo", "/img/atlas-a-logo.svg");
                // cache.Set("logo_path", "wwwroot/img/atlas-a-logo.svg");
            }
        }
        else
        {
            // cache.Set("logo", "/img/atlas-a-logo.svg");
            // cache.Set("logo_path", "wwwroot/img/atlas-a-logo.svg");
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

app.Run();

# pragma warning disable S1118
public partial class Program { }
