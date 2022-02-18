using Microsoft.EntityFrameworkCore.Migrations;

namespace BinanceBotDb.Migrations
{
    public partial class AddedCoinNameToBalanceChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "coin",
                table: "t_balance_changes",
                type: "text",
                nullable: true,
                comment: "Coin name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "coin",
                table: "t_balance_changes");
        }
    }
}
