using System.Globalization;
using MCStatusDSBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MCStatusDSBot;

public class ApplicationDbContext: DbContext
{
    public DbSet<GuildSetting> GuildSettings { get; set; } = null!;
    public DbSet<Observer> Observers { get; set; } = null!;
    public DbSet<ObserverMessage> ObserversMessages { get; set; } = null!;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        // Database.EnsureCreated();
    }
        
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var converter = new ValueConverter<CultureInfo, string>(
            v => v.ToString(),
            v => CultureInfo.GetCultureInfo(v));

        builder
            .Entity<GuildSetting>()
            .Property(e => e.Locale)
            .HasConversion(converter);
        base.OnModelCreating(builder);
    }
}