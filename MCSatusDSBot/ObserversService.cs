using System.ComponentModel;
using System.Net.Sockets;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.WebSocket;
using MCSatusDSBot.Old.Models;
using MCServerStatus;
using MCServerStatus.Models;
using Microsoft.EntityFrameworkCore;

namespace MCSatusDSBot;

public class ObserversService : DiscordClientService
{
    private IServiceProvider _provider;

    public ObserversService(DiscordSocketClient client, ILogger<DiscordClientService> logger, IServiceProvider provider)
        : base(client, logger)
    {
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation("Observers hosted service is starting");
        await Client.WaitForReadyAsync(stoppingToken);
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        using (var scope = _provider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            while (!stoppingToken.IsCancellationRequested)
            {
                Logger.LogInformation("Working with observers (count: {count})", db.Observers.Count());

                List<Task> tasks = new List<Task>();
                foreach (var observer in db.Observers.Include(o => o.GuildSetting).ToList())
                {
                    tasks.Add(UpdateObserver(observer));
                }

                await Task.WhenAll(tasks);
                await Task.Delay(10000, stoppingToken);
            }
        }
    }

    private async Task UpdateObserver(Observer observer)
    {
        var address = observer.ServerAddress.Split(':');
        var ip = address[0];
        var port = address.Length >= 2 ? short.Parse(address[1]) : (short)25565;

        Status status = null;
        try
        {
            IMinecraftPinger pinger = new MinecraftPinger(ip, port);
            status = await pinger.PingAsync();
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error when pinging server");
        }

        Logger.LogInformation("Got info about {address}: {@status}", address, status != null ? status.Description.Text : "Offline");

        List<FileAttachment> attachments = new();
        if (status != null)
        {
            var imageStream = new MemoryStream(Convert.FromBase64String(status.Favicon.Split(',')[1]));
            var attachment = new FileAttachment(imageStream, "favicon.png");
            attachments.Add(attachment);
        }
        else
        {
            var imageStream = File.OpenRead("Assets/server.png");
            var attachment = new FileAttachment(imageStream, "favicon.png");
            attachments.Add(attachment);
        }

        var embed = new EmbedBuilder()
            .WithTitle($"Server status {observer.ServerAddress}")
            .WithThumbnailUrl("attachment://favicon.png");

        if (status != null)
        {
            embed
                .WithColor(Color.Green)
                .WithDescription("Server online")
                .AddField("Online", $"{status.Players.Online}/{status.Players.Max}", true)
                .AddField("Players", string.Join(",", status.Players.Sample.Select(s => s.Name)), true)
                .AddField("Motd", status.Description.Text)
                .AddField("Version", status.Version.Name);
        }
        else
        {
            embed
                .WithColor(Color.Red)
                .WithDescription("Server offline");
        }

        embed.AddField("Last update", DateTime.Now.ToShortTimeString());

        (await Client.GetChannelAsync(observer.ChannelId) as ITextChannel)
            .ModifyMessageAsync(observer.MessageId, p =>
            {
                p.Content = "";
                p.Attachments = attachments;
                p.Embed = embed.Build();
            });
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Observers hosted service is stopping");
        return base.StopAsync(cancellationToken);
    }
}