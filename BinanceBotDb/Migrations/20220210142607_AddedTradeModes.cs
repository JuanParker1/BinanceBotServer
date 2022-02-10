using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BinanceBotDb.Migrations
{
    public partial class AddedTradeModes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "trade_mode",
                table: "t_settings");

            migrationBuilder.AlterColumn<int>(
                name: "limit_order_rate",
                table: "t_settings",
                type: "integer",
                nullable: false,
                comment: "Amount of percents from highest price to place limit order",
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Amount of % from highest price to place limit order");

            migrationBuilder.AddColumn<int>(
                name: "id_trade_mode",
                table: "t_settings",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "Selected trade mode");

            migrationBuilder.CreateTable(
                name: "t_trade_modes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    caption = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, comment: "Caption")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_trade_modes", x => x.id);
                },
                comment: "Trade modes");

            migrationBuilder.InsertData(
                table: "t_trade_modes",
                columns: new[] { "id", "caption" },
                values: new object[,]
                {
                    { 1, "Auto" },
                    { 2, "Semiauto" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_settings_id_trade_mode",
                table: "t_settings",
                column: "id_trade_mode");

            migrationBuilder.AddForeignKey(
                name: "FK_t_settings_t_trade_modes_id_trade_mode",
                table: "t_settings",
                column: "id_trade_mode",
                principalTable: "t_trade_modes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_settings_t_trade_modes_id_trade_mode",
                table: "t_settings");

            migrationBuilder.DropTable(
                name: "t_trade_modes");

            migrationBuilder.DropIndex(
                name: "IX_t_settings_id_trade_mode",
                table: "t_settings");

            migrationBuilder.DropColumn(
                name: "id_trade_mode",
                table: "t_settings");

            migrationBuilder.AlterColumn<int>(
                name: "limit_order_rate",
                table: "t_settings",
                type: "integer",
                nullable: false,
                comment: "Amount of % from highest price to place limit order",
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Amount of percents from highest price to place limit order");

            migrationBuilder.AddColumn<int>(
                name: "trade_mode",
                table: "t_settings",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "0 - trade only by signals \n 1 - purchase by signals, sale by limit order");
        }
    }
}
