using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Setting.Service.Application.Helpers;
using Setting.Service.Contract.DtoModels;
using Setting.Service.Contract.Interfaces;
using Setting.Service.Implementation;
using Xunit;
using Xunit.Abstractions;

namespace Setting.Service.xUnitTests.DataAccess
{
    public class CachingRepositoryTests : TestRunnerBase
    {
        private readonly CachingRepository _cachingRepository;

        public CachingRepositoryTests(ITestOutputHelper output) : base(output)
        {
            _cachingRepository = (CachingRepository)ServiceProvider.GetService<ICachingRepository>();
        }


        [Fact]
        public void GetDtoModulesReturnCorrectType()
        {
            Assert.NotNull(_cachingRepository);
            var res = _cachingRepository.GetDtoModulesWithCache();
            var items = Assert.IsType<List<DtoConfigurationModule>>(res);
        }

        [Fact]
        public void GetDtoModulesTestMustReturnFromCache()
        {
            Assert.NotNull(_cachingRepository);
            var res = _cachingRepository.GetDtoModulesWithCache();
            var items = Assert.IsType<List<DtoConfigurationModule>>(res);
        }


        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public void GetDtoSettingValuesByModuleIdWithCacheTest1(int moduleId)
        {
            Assert.NotNull(_cachingRepository);
            var res = _cachingRepository.GetDtoSettingValuesByModuleIdWithCache(moduleId);
            var items = Assert.IsType<List<DtoConfigurationSetting>>(res);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(1)]
        [InlineData(3)]
        public void GetDtoSettingValuesByModuleIdWithCacheTest2MustReturnFromCache(int moduleId)
        {
            Assert.NotNull(_cachingRepository);
            var res = _cachingRepository.GetDtoSettingValuesByModuleIdWithCache(moduleId);
            var items = Assert.IsType<List<DtoConfigurationSetting>>(res);
        }

        [Fact]
        public void ResetCacheTest()
        {
            Assert.NotNull(_cachingRepository);
            _cachingRepository.ResetCache();
        }


        [Theory]
        [InlineData(3, "MsmqNameDefault")]
        [InlineData(4, "TaskServiceName")]
        [InlineData(5, "TaskServiceName")]
        public void Get1SettingValueByModuleIdTest1(int moduleId, string settingName)
        {
            Assert.NotNull(_cachingRepository);
            var res = _cachingRepository.Get1SettingValueByModuleId(moduleId, settingName);
            Assert.NotNull(res);
        }

        [Theory]
        [InlineData(-1, "TaskService%20Name")]
        [InlineData(1, "Msmq%20NameDefault")]
        [InlineData(1000, "TaskService%20Name")]
        public void Get1SettingValueByModuleIdTest2MustReturnNull(int moduleId, string settingName)
        {
            Assert.NotNull(_cachingRepository);
            var res = _cachingRepository.Get1SettingValueByModuleId(moduleId, settingName);
            Assert.Null(res);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public void GetDtoSettingValuesByModuleIdWithCacheTest3MustReturnFromCache(int moduleId)
        {
            Assert.NotNull(_cachingRepository);
            var res = _cachingRepository.GetDtoSettingValuesByModuleIdWithCache(moduleId);
            var items = Assert.IsType<List<DtoConfigurationSetting>>(res);
        }
    }
}