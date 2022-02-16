using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BinanceBotDb.Migrations
{
    public partial class AddedEventLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "t_request_log",
                comment: "Application http request log",
                oldComment: "Application http requests log");

            migrationBuilder.AlterColumn<string>(
                name: "request_path",
                table: "t_request_log",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "request_method",
                table: "t_request_log",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "referer",
                table: "t_request_log",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "login",
                table: "t_request_log",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ip",
                table: "t_request_log",
                type: "character varying(25)",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "t_event_templates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    template = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true, comment: "Template text")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_event_templates", x => x.id);
                },
                comment: "Event log text templates");

            migrationBuilder.CreateTable(
                name: "t_events",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_user = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, comment: "Дата совершения события"),
                    is_read = table.Column<bool>(type: "boolean", nullable: false, comment: "Shows if event was read by user or not"),
                    text = table.Column<string>(type: "character varying(700)", maxLength: 700, nullable: true, comment: "Event text")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_t_events_t_users_id_user",
                        column: x => x.id_user,
                        principalTable: "t_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "User/application event log");

            migrationBuilder.InsertData(
                table: "t_event_templates",
                columns: new[] { "id", "template" },
                values: new object[,]
                {
                    { 1, "Совершена покупка торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}." },
                    { 2, "Совершена продажа торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}." },
                    { 3, "Произошла ошибка при попытке покупки торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.\nТекст ошибки: {}." },
                    { 4, "Произошла ошибка при попытке продажи торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.\nТекст ошибки: {}." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_events_id_user",
                table: "t_events",
                column: "id_user");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_event_templates");

            migrationBuilder.DropTable(
                name: "t_events");

            migrationBuilder.AlterTable(
                name: "t_request_log",
                comment: "Application http requests log",
                oldComment: "Application http request log");

            migrationBuilder.AlterColumn<string>(
                name: "request_path",
                table: "t_request_log",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "request_method",
                table: "t_request_log",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "referer",
                table: "t_request_log",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "login",
                table: "t_request_log",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ip",
                table: "t_request_log",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(25)",
                oldMaxLength: 25,
                oldNullable: true);
        }
    }
}
