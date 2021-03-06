// <auto-generated />
using System;
using MCStatusDSBot;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MCStatusDSBot.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.4");

            modelBuilder.Entity("MCStatusDSBot.Models.GuildSetting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("GuildId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Locale")
                        .HasColumnType("TEXT");

                    b.Property<ulong?>("NotificationChannelId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GuildId")
                        .IsUnique();

                    b.ToTable("GuildSettings");
                });

            modelBuilder.Entity("MCStatusDSBot.Models.Observer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ServerAddress")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ServerAddress")
                        .IsUnique();

                    b.ToTable("Observers");
                });

            modelBuilder.Entity("MCStatusDSBot.Models.ObserverMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("ChannelId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GuildSettingId")
                        .HasColumnType("INTEGER");

                    b.Property<ulong>("MessageId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ObserverId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GuildSettingId");

                    b.HasIndex("ObserverId");

                    b.ToTable("ObserversMessages");
                });

            modelBuilder.Entity("MCStatusDSBot.Models.ObserverMessage", b =>
                {
                    b.HasOne("MCStatusDSBot.Models.GuildSetting", "GuildSetting")
                        .WithMany()
                        .HasForeignKey("GuildSettingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MCStatusDSBot.Models.Observer", "Observer")
                        .WithMany("Messages")
                        .HasForeignKey("ObserverId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GuildSetting");

                    b.Navigation("Observer");
                });

            modelBuilder.Entity("MCStatusDSBot.Models.Observer", b =>
                {
                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}
