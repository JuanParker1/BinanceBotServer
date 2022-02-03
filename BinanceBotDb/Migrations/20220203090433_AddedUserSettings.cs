using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BinanceBotDb.Migrations
{
    public partial class AddedUserSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "patronymic",
                table: "t_users");

            migrationBuilder.CreateTable(
                name: "t_users_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_user = table.Column<int>(type: "integer", nullable: false),
                    is_trade_enabled = table.Column<bool>(type: "boolean", nullable: false, comment: "Trade on/off"),
                    trade_mode = table.Column<int>(type: "integer", nullable: false, comment: "0 - trade only by signals \n 1 - purchase by signals, sale by limit order"),
                    limit_order_rate = table.Column<int>(type: "integer", nullable: false, comment: "Amount of % from highest price to place limit order"),
                    api_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, comment: "api key"),
                    secret_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, comment: "secret key")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_users_settings", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_users_settings_t_users_id_user",
                        column: x => x.id_user,
                        principalTable: "t_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Users trade settings");

            migrationBuilder.CreateIndex(
                name: "IX_t_users_settings_id_user",
                table: "t_users_settings",
                column: "id_user",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_users_settings");

            migrationBuilder.AddColumn<string>(
                name: "patronymic",
                table: "t_users",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                comment: "Patronymic");
        }
    }
}
