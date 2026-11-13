using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Xunit;

namespace web.Tests.IntegrationTests
{
    public class AuthApiTests : IClassFixture<WebFactory<Program>>
    {
        private readonly WebFactory<Program> _factory;

        public AuthApiTests(WebFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task DemoLogin_IssuesBearerToken_ThatCanCallMe()
        {
            var client = _factory.CreateClient(
                new WebApplicationFactoryClientOptions { AllowAutoRedirect = false }
            );

            var loginResponse = await client.GetAsync("/api/auth/login");

            Assert.Equal(HttpStatusCode.Redirect, loginResponse.StatusCode);
            var redirectUrl = loginResponse.Headers.Location;
            Assert.NotNull(redirectUrl);

            var token = QueryHelpers.ParseQuery(redirectUrl.Query)["token"].ToString();
            Assert.False(string.IsNullOrWhiteSpace(token));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );

            var meResponse = await client.GetAsync("/api/auth/me");

            var payload = await meResponse.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
            Assert.Contains("\"username\":\"Default\"", payload);
            Assert.Contains("\"userId\":\"1\"", payload);
            Assert.Contains("\"adminEnabled\":", payload);
        }

        [Fact]
        public async Task GetMe_WithoutBearerToken_ReturnsUnauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/auth/me");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
