using System.Threading.Tasks;
using Setting.Service.Application;
using Setting.Service.Common;
using Setting.Service.Contract;
using Setting.Service.Tests.Helpers;
using Xunit;

namespace Setting.Service.Tests.IntegrationTests
{
    public class SwaggerTests : IClassFixture<MoqWebApplicationFactory<Startup>>
    {
        private readonly MoqWebApplicationFactory<Startup> _factory;

        public SwaggerTests(MoqWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_ServiceReturnInformationPage()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());

            var swaggerUi = await HtmlHelpers.GetDocumentAsync(response);
            Assert.Equal(AppConstants.SwaggerDocumentTitle, swaggerUi.Title);
        }

        [Theory]
        [InlineData("/swagger/v1/swagger.json")]
        public async Task Get_ServiceReturnSwaggerDescription(string url)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

    }
}
