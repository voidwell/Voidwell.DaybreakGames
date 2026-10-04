using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Voidwell.DaybreakGames.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PS2DbContext>
{
    public PS2DbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? "Development";

        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var builder = new DbContextOptionsBuilder<PS2DbContext>();

        var connectionString = configuration.GetValue<string>("DBConnectionString");

        builder.UseNpgsql(connectionString, o =>
        {
            o.CommandTimeout(7200);
        });

        return new PS2DbContext(builder.Options);
    }
}
