using System.Linq;
using MCStatusDSBot.Old.Models;
using Microsoft.EntityFrameworkCore;

namespace MCStatusDSBot.Extensions;

public static class DbSetExtension
{
    public static GuildSetting GetOrCreate(this DbSet<GuildSetting> source, ulong guildId)
    {
        var settings = source.FirstOrDefault(s => s.GuildId == guildId);
        if (settings == null)
        {
            settings = new GuildSetting {GuildId = guildId};
            source.Add(settings);
        }

        return settings;
    }
    
    public static Observer GetOrCreate(this DbSet<Observer> source, string address)
    {
        var observer = source.FirstOrDefault(o => o.ServerAddress == address);
        if (observer == null)
        {
            observer = new Observer {ServerAddress = address};
            source.Add(observer);
        }

        return observer;
    }
}