using MCSatusDSBot.Old.Models;
using Microsoft.EntityFrameworkCore;

namespace MCSatusDSBot;

public class ApplicationDbContext: DbContext
{
    public DbSet<GuildSetting> GuildSettings { get; set; } = null!;
    public DbSet<Observer> Observers { get; set; } = null!;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
        
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}