using System.Diagnostics;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MCSatusDSBot.Old.Models;

namespace MCSatusDSBot.Old.Modules;

[Group("minestatus", "Minecraft server status")]
public class MinecraftStatusModule : InteractionModuleBase<SocketInteractionContext>
{
    public InteractionService Commands { get; set; }
    public IServiceProvider ServiceProvider { get; set; }
    public ILogger<MinecraftStatusModule> Logger { get; set; }
    public ApplicationDbContext Db { get; set; }
    

    [RequireUserPermission(GuildPermission.Administrator)]
    [SlashCommand("watch", "Add server to watch")]
    public async Task Watch([Summary(description: "Server address")] string address)
    {
        await DeferAsync(true);

        var message = await Context.Channel.SendMessageAsync("Loading");

        ((await Context.Client.GetChannelAsync(message.Channel.Id)) as ITextChannel).ModifyMessageAsync(message.Id, properties => properties.Content = "a");

        await FollowupAsync("Observer created!", ephemeral: true);
    }

    [RequireUserPermission(GuildPermission.Administrator)]
    [SlashCommand("setnotificationchannel", "Set notification cahnnel")]
    public async Task SetChannel(ITextChannel channel)
    {
        await DeferAsync();
        var settings = Db.GuildSettings.FirstOrDefault(s => s.GuildId == Context.Guild.Id);
        if (settings == null)
        {
            settings = new GuildSetting {GuildId = Context.Guild.Id};
            Db.GuildSettings.Add(settings);
            Db.SaveChanges();
        }
        
        settings.NotificationChannelId = channel.Id;
        Db.SaveChanges();
        
        Logger.LogInformation("Guild {guildName} ({guildId}): Notification channel changed to {channelName}", Context.Guild.Name, Context.Guild.Id, channel.Name);

        await FollowupAsync($"Notification channel changed to {channel.Name}");
    }

    [SlashCommand("test", "Test")]
    public async Task Test([Autocomplete] string str)
    {
        var builder = new ComponentBuilder().WithButton("Test", "myButton");
        
        await RespondAsync($"Reply {str}", components:builder.Build());
    }

    [ComponentInteraction("myButton", true)]
    public async Task ClickButtonAsync()
    {
        Console.WriteLine("Test");
        await RespondAsync(text: ":thumbsup: Clicked!" + Context.User.Username);
    } 
    
    [AutocompleteCommand("str", "test")]
    public async Task Autocomplete()
    {
        Console.WriteLine("run");
        List<AutocompleteResult> results = new List<AutocompleteResult>()
        {
            new ("Test", "Test"),
            new ("Test2", "Test2")
        };
        await (Context.Interaction as SocketAutocompleteInteraction).RespondAsync(results);
    }
}