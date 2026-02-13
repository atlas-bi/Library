using System.Text;
using System.Text.Json;
using Atlas_Web.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Atlas_Web.Pages.Requests
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _client;

        public IndexModel(IConfiguration config)
        {
            _config = config;

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
            };

            _client = new HttpClient(handler)
            {
                DefaultRequestVersion = new Version(1, 1),
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact,
            };

            _client.DefaultRequestHeaders.Add("Accept", "application/vnd.manageengine.sdp.v3+json");
            _client.DefaultRequestHeaders.Add(
                "authtoken",
                _config["AppSettings:manage_engine_tech_key"]
            );
        }

        public void OnGet() { }

        // ---------------------------
        // PUBLIC ACTIONS
        // ---------------------------

        public async Task<ActionResult> OnPostAccessRequest(
            string reportName,
            string reportUrl,
            string directorName
        )
        {
            var request = BuildBaseRequest(
                subject: "Atlas Access Request",
                description: $"I would like access to '{reportName}' from {reportUrl}",
                itemName: "Request Access",
                udfFields: new
                {
                    udf_sline_4517 = directorName,
                    udf_sline_5791 = reportName,
                    udf_sline_5790 = reportUrl,
                }
            );

            return await SendRequestAsync(request);
        }

        public async Task<ActionResult> OnPostShareFeedback(
            string reportName,
            string reportUrl,
            string description
        )
        {
            var request = BuildBaseRequest(
                subject: "Atlas Feedback",
                description: $"<b>Atlas feedback on <a href='{reportUrl}'>{reportName}</a></b><br/><br/><p>{description}</p>",
                itemName: "Feedback",
                udfFields: new { udf_sline_5791 = reportName, udf_sline_5790 = reportUrl }
            );

            return await SendRequestAsync(request);
        }

        // ---------------------------
        // SHARED HELPERS
        // ---------------------------

        private object BuildBaseRequest(
            string subject,
            string description,
            string itemName,
            object udfFields
        )
        {
            return new
            {
                request = new
                {
                    subject,
                    description,
                    requester = new { email_id = User.GetUserEmail() },
                    template = new { name = "WebAPI" },
                    status = new { name = "Open" },
                    category = new { name = "Epic Request" },
                    subcategory = new { name = "Atlas" },
                    item = new { name = itemName },
                    udf_fields = udfFields,
                },
            };
        }

        private async Task<ActionResult> SendRequestAsync(object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            var content = new FormUrlEncodedContent(
                new Dictionary<string, string> { { "input_data", json } }
            );

            var url = _config["AppSettings:manage_engine_server"] + "/api/v3/requests";

            try
            {
                var response = await _client.PostAsync(url, content);
                var body = await response.Content.ReadAsStringAsync();

                return Content(
                    $"Status Code: {(int)response.StatusCode}\nResponse Body:\n{body}",
                    "application/json"
                );
            }
            catch (Exception ex)
            {
                return Content($"HTTP request failed: {ex}");
            }
        }
    }
}
