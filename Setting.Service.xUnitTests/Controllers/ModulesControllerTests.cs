using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Setting.Service.Application.Controllers;
using Setting.Service.Contract.DtoModels;
using Setting.Service.Contract.Interfaces;
using Setting.Service.xUnitTests;
using Xunit;
using Xunit.Abstractions;

// ReSharper disable once CheckNamespace
namespace Setting.Service.Controllers.xUnitTests
{


    public class ModulesControllerTests : TestRunnerBase
    {
        private readonly ICachingRepository _cachingRepository;
        private readonly ModulesController _controller;


        public ModulesControllerTests(ITestOutputHelper output) : base(output)
        {
            _cachingRepository = ServiceProvider.GetService<ICachingRepository>();
            _controller = new ModulesController(LoggerFactory, _cachingRepository);
        }


        [Theory]
        [InlineData(5, true)]
        [InlineData(5, false)]
        [InlineData(1000, true)]
        public void GetSettingValuesByModuleId_ReturnValuesFromCacheAndRepository(int moduleId, bool useCache)
        {
            var okResult = _controller.GetSettingValuesByModuleId(moduleId, useCache).Result as OkObjectResult;
            Assert.NotNull(okResult);
            var items = Assert.IsAssignableFrom<IList<DtoConfigurationSetting>>(okResult.Value);
            Assert.All(items, item => Assert.NotNull(item.Name));
            Output.WriteLine("Settings count : {0}", items.Count());

            var envVariable = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Output.WriteLine($"env: '{envVariable}'");
        }


        //[Fact()]
        //public void GetSettingValuesByModuleId_RiseErrorWhenCacheNotInitialized()
        //{
        //    var noCacheController = new ModulesController(_loggerFactory, _cachingRepository);
        //    Assert.Throws<InvalidOperationException>(() =>
        //    {
        //        var result = noCacheController.GetSettingValuesByModuleId(5, false).Result;
        //    });
        //}


        [Fact]
        public void ModulesListAreNotEmpty()
        {
            Assert.NotNull(_cachingRepository);
            var okResult = _controller.GetModules().Result as OkObjectResult;
            Assert.NotNull(okResult);
            var items = Assert.IsType<List<DtoConfigurationModule>>(okResult.Value);
            Assert.Contains(items, item => item.Id == 5);
        }

        [Fact]
        public void ControllerLogErrorsAndTrowException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var badController = new ModulesController(LoggerFactory, null);
                ActionResult result = badController.GetModules().Result as OkObjectResult;
            });
        }


        [Theory]
        [InlineData(3, "MsmqNameDefault")]
        [InlineData(4, "TaskServiceName")]
        [InlineData(5, "TaskServiceName")]
        public void GetSettingValueByModuleId_ReturnCorrectType(int moduleId, string settingName)
        {
            var okResult = _controller.GetSettingValueByModuleId(moduleId, settingName).Result as OkObjectResult;
            Assert.NotNull(okResult);
            var item = Assert.IsType<string>(okResult.Value);
            Assert.NotNull(item);
        }


        [Theory]
        [InlineData(-1, "TaskService%20Name")]
        [InlineData(1, "Msmq%20NameDefault")]
        [InlineData(1000, "TaskService%20Name")]
        public void GetSettingValueByModuleId_ReturnCorrectValue(int moduleId, string settingName)
        {
            var okResult = _controller.GetSettingValueByModuleId(moduleId, settingName).Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Null(okResult.Value);
        }
    }
}