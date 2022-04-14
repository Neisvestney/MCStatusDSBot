using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using Discord;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.Net;
using Discord.WebSocket;
using MCServerStatus;
using MCServerStatus.Models;
using MCStatusDSBot.Old.Models;
using Microsoft.EntityFrameworkCore;

namespace MCStatusDSBot;

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
                foreach (var observer in db.Observers.Include(o => o.Messages).ToList())
                {
                    tasks.Add(UpdateObserver(observer, db));
                }

                await Task.WhenAll(tasks);
                Logger.LogInformation("All done!");
                await Task.Delay(10000, stoppingToken);
            }
        }
    }

    private async Task UpdateObserver(Observer observer, ApplicationDbContext db)
    {
        try
        {
            var address = observer.ServerAddress.Split(':');
            var ip = address[0];
            var port = address.Length >= 2 ? short.Parse(address[1]) : (short)25565;
            
            Logger.LogDebug("Pinging {address} (id: {observer})", observer.ServerAddress, observer.Id);

            Status status = null;
            try
            {
                IMinecraftPinger pinger = new MinecraftPinger(ip, port);
                status = await pinger.PingAsync();
            }
            catch (SocketException e) when(e.ErrorCode == 11001 || e.ErrorCode == 10060) {}
            catch (Exception e)
            {
                Logger.LogWarning(e, "Error when pinging server");
            }

            Logger.LogInformation("Got info about {address} (id: {observer}): {status}",
                observer.ServerAddress, observer.Id, status != null ? status.Description.Text : "Offline");

            var embed = new EmbedBuilder()
                .WithTitle($"Server status {observer.ServerAddress}")
                .WithThumbnailUrl("attachment://favicon.png");

            if (status != null)
            {
                embed
                    .WithColor(Color.Green)
                    .WithDescription("Server online")
                    .AddField("Online", $"{status.Players.Online}/{status.Players.Max}", true)
                    .AddField("Players",
                        status.Players.Sample != null
                            ? string.Join(",", status.Players.Sample.Select(s => s.Name))
                            : "No players", true)
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

            foreach (var message in observer.Messages.ToArray())
            {
                Logger.LogDebug("Updating message {@message}[{@channel}] (id: {id}, observer id: {observer})", message.MessageId, observer.Id,
                    message.ChannelId, message.Id);

                try
                {
                    await (await Client.GetChannelAsync(message.ChannelId) as ITextChannel)
                        .ModifyMessageAsync(message.MessageId, p =>
                        {
                            Stream imageStream;
                            if (status != null)
                                imageStream = new MemoryStream(Convert.FromBase64String(status.Favicon.Split(',')[1]));
                            else 
                                imageStream = File.OpenRead("Assets/server.png");
                            List<FileAttachment> attachments = new() {new FileAttachment(imageStream, "favicon.png")};

                            p.Content = "";
                            p.Attachments = attachments;
                            p.Embed = embed.Build();
                        });
                    
                    Logger.LogInformation("Message {@message}[{@channel}] (id: {id}, observer id: {observer}) updated", message.MessageId, message.ChannelId, message.Id, observer.Id);
                }
                catch (HttpException e) when (e.DiscordCode == DiscordErrorCode.UnknownMessage)
                {
                    Logger.LogInformation("Message (id: {message}) for observer {observer} (id: {id}) deleted",
                        message.Id, observer.ServerAddress, observer.Id);
                    db.ObserversMessages.Remove(message);
                    db.SaveChanges();

                    // observer = db.Observers.First(o => o.Id == observer.Id);
                    if (observer.Messages.Count() == 0)
                    {
                        Logger.LogInformation("Observer {observer} (id: {id}) deleted", observer.ServerAddress,
                            observer.Id);
                        db.Observers.Remove(observer);
                    }

                    db.SaveChanges();
                }
            }
        }
        catch (Exception e)
        {
            Logger.LogCritical(e, "Unexpected error while observer updating");
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        Logger.LogInformation("Observers hosted service is stopping");
        return base.StopAsync(cancellationToken);
    }
}