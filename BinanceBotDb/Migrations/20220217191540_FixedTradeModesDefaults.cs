using Microsoft.EntityFrameworkCore.Migrations;

namespace BinanceBotDb.Migrations
{
    public partial class FixedTradeModesDefaults : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "t_trade_modes",
                keyColumn: "id",
                keyValue: 1,
                column: "caption",
                value: "auto");

            migrationBuilder.UpdateData(
                table: "t_trade_modes",
                keyColumn: "id",
                keyValue: 2,
                column: "caption",
                value: "semiAuto");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "t_trade_modes",
                keyColumn: "id",
                keyValue: 1,
                column: "caption",
                value: "Auto");

            migrationBuilder.UpdateData(
                table: "t_trade_modes",
                keyColumn: "id",
                keyValue: 2,
                column: "caption",
                value: "Semiauto");
        }
    }
}
