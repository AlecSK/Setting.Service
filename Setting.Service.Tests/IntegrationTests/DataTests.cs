using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Setting.Service.Application;
using Setting.Service.Contract.DtoModels;
using Xunit;

namespace Setting.Service.Tests.IntegrationTests
{
    public class DataTests : IClassFixture<MoqWebApplicationFactory<Startup>>
    {
        readonly HttpClient _client;


        public DataTests(MoqWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Add("ContentType", "application/json");
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes("user1:password1");
            string val = System.Convert.ToBase64String(plainTextBytes);
            _client.DefaultRequestHeaders.Add("Authorization", "Basic " + val);
        }

        [Fact]
        public async Task Get_EndpointModulesReturnCorrectType()
        {
            var response = await _client.GetAsync("/Modules");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var res = response.Content.ReadAsStringAsync().Result;
            Trace.WriteLine(res);
            var deserialized = JsonConvert.DeserializeObject<IEnumerable<DtoConfigurationModule>>(res);
            Assert.True(deserialized.Count() >= 5);
        }


        [Theory]
        [InlineData("/Modules")]
        [InlineData("/modules/1/true/settings/values")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
            var res = response.Content.ReadAsStringAsync().Result;
        }

        [Theory]
        [InlineData("/modules/1/settings/DefaultCacheLifeTimeInMinutes/value")]
        [InlineData("/modules/10/settings/MsmqNameDefault/value")]
        [InlineData("/Cache/Reset")]
        public async Task Get_EndpointsReturnSuccess(string url)
        {

            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }

        [Theory]
        [InlineData("/modules/1/settings/DefaultCacheLife%20TimeInMinutes/value")]
        [InlineData("/modules/1000/settings/DefaultCacheLifeTimeInMinutes/value")]
        public async Task Get_EndpointsReturnSuccessButNullData(string url)
        {
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var res = response.Content.ReadAsStringAsync().Result;
            Assert.True(string.IsNullOrEmpty(res));
        }
    }
}
