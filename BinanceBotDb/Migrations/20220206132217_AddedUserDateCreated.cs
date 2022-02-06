using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BinanceBotDb.Migrations
{
    public partial class AddedUserDateCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_users_settings");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "t_users",
                newName: "password");

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created",
                table: "t_users",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "t_settings",
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
                    table.PrimaryKey("PK_t_settings", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_settings_t_users_id_user",
                        column: x => x.id_user,
                        principalTable: "t_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Application trade settings");

            migrationBuilder.CreateIndex(
                name: "IX_t_settings_id_user",
                table: "t_settings",
                column: "id_user",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_settings");

            migrationBuilder.DropColumn(
                name: "date_created",
                table: "t_users");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "t_users",
                newName: "password_hash");

            migrationBuilder.CreateTable(
                name: "t_users_settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    api_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, comment: "api key"),
                    id_user = table.Column<int>(type: "integer", nullable: false),
                    is_trade_enabled = table.Column<bool>(type: "boolean", nullable: false, comment: "Trade on/off"),
                    limit_order_rate = table.Column<int>(type: "integer", nullable: false, comment: "Amount of % from highest price to place limit order"),
                    secret_key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, comment: "secret key"),
                    trade_mode = table.Column<int>(type: "integer", nullable: false, comment: "0 - trade only by signals \n 1 - purchase by signals, sale by limit order")
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
    }
}
