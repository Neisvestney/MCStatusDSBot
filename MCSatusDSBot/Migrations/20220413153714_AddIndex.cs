using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MCSatusDSBot.Migrations
{
    public partial class AddIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Observers_ServerAddress",
                table: "Observers",
                column: "ServerAddress",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Observers_ServerAddress",
                table: "Observers");
        }
    }
}
