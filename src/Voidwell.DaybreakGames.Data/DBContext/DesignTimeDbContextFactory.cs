/*
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Voidwell.DaybreakGames.Data.DBContext
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PS2DbContext>
    {
        public PS2DbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("testsettings.json")
                .Build();

            var options = configuration.Get<DatabaseOptions>();

            return new PS2DbContext(options);
        }
    }
}
*/