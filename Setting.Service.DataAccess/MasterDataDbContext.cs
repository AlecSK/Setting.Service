using Microsoft.EntityFrameworkCore;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Setting.Service.DataAccess
{
    public class MasterDataDbContext : DbContext
    {
        public MasterDataDbContext(DbContextOptions<MasterDataDbContext> options) : base(options) { }


        public DbSet<ConfigurationModule> Modules { get; set; }

        public DbSet<ConfigurationSetting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
