namespace MCSatusDSBot.Old.Models;

public class Observer
{
    public int Id { get; set; }
    public GuildSetting GuildSetting { get; set; }
    
    public ulong ChannelId { get; set; }
    public ulong MessageId { get; set; }
    public string ServerAddress { get; set; }
}