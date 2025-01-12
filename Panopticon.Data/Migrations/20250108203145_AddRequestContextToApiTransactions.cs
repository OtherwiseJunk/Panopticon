using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Panopticon.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestContextToApiTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionData",
                table: "ApiTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionData",
                table: "ApiTransactions");
        }
    }
}
