namespace MCSatusDSBot.Old.Models;

public class ObserverMessage
{
    public int Id { get; set; }
    public GuildSetting GuildSetting { get; set; }
    public Observer Observer { get; set; }
    
    public ulong ChannelId { get; set; }
    public ulong MessageId { get; set; }
}