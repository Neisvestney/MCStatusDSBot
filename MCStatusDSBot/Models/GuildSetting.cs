using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace MCStatusDSBot.Models;

[Index(nameof(GuildId), IsUnique = true)]
public class GuildSetting
{
    public int Id { get; set; }
    public ulong GuildId { get; set; }
    
    public ulong? NotificationChannelId { get; set; }
    public CultureInfo? Locale { get; set; }
}