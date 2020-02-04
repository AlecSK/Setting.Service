using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Setting.Service.DataAccess;
using Xunit;
using Xunit.Abstractions;

namespace Setting.Service.xUnitTests.DataAccess
{
    public class MasterDataDbContextTests: TestRunnerBase
    {
        private readonly MasterDataDbContext _db;

        public MasterDataDbContextTests(ITestOutputHelper output) : base(output)
        {
            _db = ServiceProvider.GetService<MasterDataDbContext>();
        }

        [Fact]
        public void ModulesMayHaveSettings()
        {
            var module = _db.Modules.First(t => t.Settings.Any());
            Assert.NotNull(module);
            _db.Entry(module).Collection(t => t.Settings).Load();
            var items = Assert.IsAssignableFrom<IEnumerable<ConfigurationSetting>>(module.Settings);
            Assert.NotEmpty(items);
        }

        [Fact]
        public void ConfigurationSettingMayHaveParentModule()
        {
            var setting = _db.Settings.First(t => t.ConfigurationModule != null);
            Assert.NotNull(setting);
            _db.Entry(setting).Reference(t => t.ConfigurationModule).Load();
            var item = Assert.IsType<ConfigurationModule>(setting.ConfigurationModule);

        }
    }
}