using System.Threading.Tasks;
using Atlas_Web.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace web.Tests.IntegrationTests
{
    public class BasicTests : IClassFixture<SsoWebFactory<Program>>
    {
        private readonly SsoWebFactory<Program> _factory;

        public BasicTests(SsoWebFactory<Program> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Collections")]
        [InlineData("/Collections/New")]
        [InlineData("/Initiatives")]
        [InlineData("/Initiatives/New")]
        [InlineData("/Terms")]
        [InlineData("/Terms/New")]
        [InlineData("/Search")]
        [InlineData("/Search?Query=test")] // > need to have a solr install for this to
        [InlineData("/Settings")]
        [InlineData("/Analytics")]
        [InlineData("/Users")] // me
        [InlineData("/Users?id=2")] // you
        [InlineData("/tasks")]
        //[InlineData("/Groups")] > requires an id
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(TestSsoSchemeOptions.HeaderName, "Default");

            // Act
            var response = await client.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            // Assert
            if (!response.IsSuccessStatusCode)
            {
                throw new System.Exception(
                    $"Request to '{url}' failed with {(int)response.StatusCode} ({response.StatusCode}). Body:\n{body}"
                );
            }
            // Assert.Equal(
            //     "text/html; charset=utf-8",
            //     response.Content.Headers.ContentType.ToString()
            // );
        }
    }
}
