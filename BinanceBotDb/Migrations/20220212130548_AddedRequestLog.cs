using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BinanceBotDb.Migrations
{
    public partial class AddedRequestLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "amount",
                table: "t_balance_changes",
                type: "double precision",
                nullable: false,
                comment: "Amount of change in USDT",
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Amount of change in USDT");

            migrationBuilder.CreateTable(
                name: "t_request_log",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_user = table.Column<int>(type: "integer", nullable: false),
                    login = table.Column<string>(type: "text", nullable: true),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ip = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    request_method = table.Column<string>(type: "text", nullable: true),
                    request_path = table.Column<string>(type: "text", nullable: true),
                    referer = table.Column<string>(type: "text", nullable: true),
                    elapsed_ms = table.Column<long>(type: "bigint", nullable: false),
                    ex_message = table.Column<string>(type: "text", nullable: true),
                    ex_stack = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_request_log", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_request_log_t_users_id_user",
                        column: x => x.id_user,
                        principalTable: "t_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Application http requests log");

            migrationBuilder.CreateIndex(
                name: "IX_t_request_log_id_user",
                table: "t_request_log",
                column: "id_user");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_request_log");

            migrationBuilder.AlterColumn<int>(
                name: "amount",
                table: "t_balance_changes",
                type: "integer",
                nullable: false,
                comment: "Amount of change in USDT",
                oldClrType: typeof(double),
                oldType: "double precision",
                oldComment: "Amount of change in USDT");
        }
    }
}
