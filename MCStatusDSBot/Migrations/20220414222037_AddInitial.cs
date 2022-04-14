using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCStatusDSBot.Migrations
{
    public partial class AddInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    NotificationChannelId = table.Column<ulong>(type: "INTEGER", nullable: true),
                    Locale = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Observers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ServerAddress = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Observers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ObserversMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildSettingId = table.Column<int>(type: "INTEGER", nullable: false),
                    ObserverId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChannelId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MessageId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObserversMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ObserversMessages_GuildSettings_GuildSettingId",
                        column: x => x.GuildSettingId,
                        principalTable: "GuildSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ObserversMessages_Observers_ObserverId",
                        column: x => x.ObserverId,
                        principalTable: "Observers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GuildSettings_GuildId",
                table: "GuildSettings",
                column: "GuildId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Observers_ServerAddress",
                table: "Observers",
                column: "ServerAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ObserversMessages_GuildSettingId",
                table: "ObserversMessages",
                column: "GuildSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_ObserversMessages_ObserverId",
                table: "ObserversMessages",
                column: "ObserverId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObserversMessages");

            migrationBuilder.DropTable(
                name: "GuildSettings");

            migrationBuilder.DropTable(
                name: "Observers");
        }
    }
}
