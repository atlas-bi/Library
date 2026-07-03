using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Atlas_Web.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Xunit;

namespace web.Tests.IntegrationTests
{
    public class SsoAuthApiTests
    {
        [Fact]
        public async Task Login_TestSsoUser_IssuesBearerToken_ThatCanCallMe()
        {
            using var factory = new SsoWebFactory<Program>();
            var client = factory.CreateClient(
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
            );
            client.DefaultRequestHeaders.Add(TestSsoSchemeOptions.HeaderName, "Default");

            var loginResponse = await client.GetAsync("/api/auth/login");

            Assert.Equal(HttpStatusCode.Redirect, loginResponse.StatusCode);
            var redirectUrl = loginResponse.Headers.Location;
            Assert.NotNull(redirectUrl);

            var token = QueryHelpers.ParseQuery(redirectUrl.Query)["token"].ToString();
            Assert.False(string.IsNullOrWhiteSpace(token));

            client.DefaultRequestHeaders.Remove(TestSsoSchemeOptions.HeaderName);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );

            var meResponse = await client.GetAsync("/api/auth/me");

            Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
            var payload = await meResponse.Content.ReadAsStringAsync();
            Assert.Contains("\"username\":\"Default\"", payload);
            Assert.Contains("\"userId\":\"1\"", payload);
        }
    }
}
