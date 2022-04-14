using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using MCStatusDSBot;
using Microsoft.EntityFrameworkCore;
using Sample.Serilog;
using Serilog;
using Serilog.Events;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
    .WriteTo.Console(
        restrictedToMinimumLevel: LogEventLevel.Information)
    .WriteTo.File("log.txt",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true)
    .CreateLogger();

try
{
    Log.Information("Starting host");

    var host = Host.CreateDefaultBuilder(args)
        // Serilog.Extensions.Hosting is required.
        .UseSerilog()
        .ConfigureDiscordHost((context, config) =>
        {
            config.SocketConfig = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 200
            };

            config.Token = context.Configuration["Token"];

            config.LogFormat = (message, exception) => $"{message.Source}: {message.Message}";
        })
        .UseInteractionService((context, config) =>
        {
            config.LogLevel = LogSeverity.Info;
            config.UseCompiledLambda = true;
        })
        .ConfigureServices((context, services) =>
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Filename=DataBase.db"));
            services.AddHostedService<InteractionHandler>();
            services.AddHostedService<ObserversService>();
        }).Build();

    await host.RunAsync();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}