using Voidwell.DaybreakGames.Api;
using Voidwell.DaybreakGames.Api.Logging;

var builder = Host.CreateDefaultBuilder();

builder.ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder
        .UseStartup<Startup>()
        .UseUrls("http://0.0.0.0:5000");
});

builder.ConfigureLogging((context, logging) =>
{
    logging.AddApiLogging(context.HostingEnvironment, context.Configuration);
});

var app = builder.Build();
await app.RunAsync();
