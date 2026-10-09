using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Voidwell.DaybreakGames.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PS2DbContext>
{
    public PS2DbContext CreateDbContext(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile(Path.Combine("..", "Voidwell.DaybreakGames.Api", "appsettings.json"), optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile(Path.Combine("..", "Voidwell.DaybreakGames.Api", "appsettings.Development.json"), optional: true)
            .AddEnvironmentVariables()
            .Build();

        var options = configuration.Get<DatabaseOptions>() ?? new DatabaseOptions();
        options.CommandTimeout ??= 180;

        return new PS2DbContext(PS2DbContext.CreateDefaultDbContextOptions(options));
    }
}
