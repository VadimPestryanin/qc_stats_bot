using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace stats_eba_bot.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerStatistic",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PlayerName = table.Column<string>(nullable: true),
                    DuelRating = table.Column<int>(nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStatistic", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerStatistic");
        }
    }
}
