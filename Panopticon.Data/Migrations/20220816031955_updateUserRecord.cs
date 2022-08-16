using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Panopticon.Data.Migrations
{
    public partial class updateUserRecord : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastTimePosted",
                table: "UserRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<double>(
                name: "LibcraftCoinBalance",
                table: "UserRecords",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "TimeOut",
                table: "UserRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastTimePosted",
                table: "UserRecords");

            migrationBuilder.DropColumn(
                name: "LibcraftCoinBalance",
                table: "UserRecords");

            migrationBuilder.DropColumn(
                name: "TimeOut",
                table: "UserRecords");
        }
    }
}
