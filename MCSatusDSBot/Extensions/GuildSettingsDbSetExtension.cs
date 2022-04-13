using MCSatusDSBot.Old.Models;
using Microsoft.EntityFrameworkCore;

namespace MCSatusDSBot.Extensions;

public static class GuildSettingsDbSetExtension
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
}