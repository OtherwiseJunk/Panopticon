using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Panopticon.Data.Migrations
{
    public partial class AddOOCItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutOfContextItems",
                columns: table => new
                {
                    ItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportingUserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiscordGuildId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    DateStored = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutOfContextItems", x => x.ItemID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutOfContextItems");
        }
    }
}
