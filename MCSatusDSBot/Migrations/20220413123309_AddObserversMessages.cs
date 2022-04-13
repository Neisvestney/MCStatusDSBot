using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCSatusDSBot.Migrations
{
    public partial class AddObserversMessages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Observers_GuildSettings_GuildSettingId",
                table: "Observers");

            migrationBuilder.DropIndex(
                name: "IX_Observers_GuildSettingId",
                table: "Observers");

            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "Observers");

            migrationBuilder.DropColumn(
                name: "GuildSettingId",
                table: "Observers");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "Observers");

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

            migrationBuilder.AddColumn<ulong>(
                name: "ChannelId",
                table: "Observers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.AddColumn<int>(
                name: "GuildSettingId",
                table: "Observers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<ulong>(
                name: "MessageId",
                table: "Observers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0ul);

            migrationBuilder.CreateIndex(
                name: "IX_Observers_GuildSettingId",
                table: "Observers",
                column: "GuildSettingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Observers_GuildSettings_GuildSettingId",
                table: "Observers",
                column: "GuildSettingId",
                principalTable: "GuildSettings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
