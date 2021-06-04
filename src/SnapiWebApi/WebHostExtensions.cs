using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SnapiCore.Data;

namespace SnapiWebApi
{
    public static class WebHostExtensions
    {
        public static async Task RunApplicationAsync(this IHostBuilder builder)
        {
            await builder.Build().RunApplicationAsync();
        }

        public static async Task RunApplicationAsync(this IHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var migrator = scope.ServiceProvider.GetRequiredService<SnapiDbContext>();
                await migrator.Database.MigrateAsync();
            }

            await webHost.RunAsync();
        }
    }
}