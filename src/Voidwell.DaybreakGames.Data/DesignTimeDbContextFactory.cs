﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Voidwell.DaybreakGames.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PS2DbContext>
    {
        public PS2DbContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<PS2DbContext>();

            var connectionString = configuration.GetValue<string>("DBConnectionString");

            builder.UseNpgsql(connectionString);

            return new PS2DbContext(builder.Options);
        }
    }
}
