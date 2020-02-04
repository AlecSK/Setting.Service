using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Setting.Service.Application.Controllers;
using Setting.Service.Contract.Interfaces;
using Setting.Service.xUnitTests;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable once CheckNamespace
namespace Setting.Service.Controllers.xUnitTests
{
    public class CacheControllerTests : TestRunnerBase
    {
        private readonly CacheController _controller;

        public CacheControllerTests(ITestOutputHelper output) : base(output)
        {
            var cachingRepository = ServiceProvider.GetService<ICachingRepository>();
            _controller = new CacheController(LoggerFactory, cachingRepository);
        }

        [Fact]
        public void CacheResetTest()
        {
            IActionResult actionResult = _controller.CacheReset();
            Assert.IsType<OkResult>(actionResult);
        }
    }
}