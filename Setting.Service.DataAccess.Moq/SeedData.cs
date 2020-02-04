using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Setting.Service.DataAccess.Moq
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MasterDataDbContext(serviceProvider.GetRequiredService<DbContextOptions<MasterDataDbContext>>()))
            {
                MoqDb.InitializeDbForTests(context);
            }

        }
    }
}
