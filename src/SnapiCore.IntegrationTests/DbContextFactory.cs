using Microsoft.EntityFrameworkCore;
using SnapiCore.Data;

namespace SnapiCore.IntegrationTests
{
    public class DbContextFactory
    {
        public static SnapiDbContext Get()
        {
            var snapiDbContext = SnapiDbContext.Build();
            snapiDbContext.Database.MigrateAsync();
            return snapiDbContext;
        }
    }
}