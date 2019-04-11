using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace stats_eba_bot.Migrations
{
    public partial class LastDuelTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastDuelPlayed",
                table: "PlayerStatistic",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastDuelPlayed",
                table: "PlayerStatistic");
        }
    }
}
