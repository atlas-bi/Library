using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
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
        public async Task GetMe_WithoutBearerToken_ReturnsUnauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/auth/me");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
