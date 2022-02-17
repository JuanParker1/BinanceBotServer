using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BinanceBotDb.Migrations
{
    public partial class RemovedErrorRequestsLogging : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ex_message",
                table: "t_request_log");

            migrationBuilder.DropColumn(
                name: "ex_stack",
                table: "t_request_log");

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "t_request_log",
                type: "integer",
                nullable: false,
                comment: "Request http status",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "request_path",
                table: "t_request_log",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                comment: "Request path",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "request_method",
                table: "t_request_log",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                comment: "Request http method",
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "referer",
                table: "t_request_log",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                comment: "Request referer",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "login",
                table: "t_request_log",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                comment: "Request user login",
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ip",
                table: "t_request_log",
                type: "character varying(25)",
                maxLength: 25,
                nullable: true,
                comment: "Request ip address",
                oldClrType: typeof(string),
                oldType: "character varying(25)",
                oldMaxLength: 25,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id_user",
                table: "t_request_log",
                type: "integer",
                nullable: false,
                comment: "Request user id",
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "elapsed_ms",
                table: "t_request_log",
                type: "bigint",
                nullable: false,
                comment: "Request lifetime",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date",
                table: "t_request_log",
                type: "timestamp without time zone",
                nullable: false,
                comment: "Request date",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.UpdateData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 3,
                column: "template",
                value: "На бирже установлен лимитный ордер для торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.");

            migrationBuilder.UpdateData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 4,
                column: "template",
                value: "На бирже отменен лимитный ордер для торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.");

            migrationBuilder.InsertData(
                table: "t_event_templates",
                columns: new[] { "id", "template" },
                values: new object[,]
                {
                    { 5, "Произошла ошибка при попытке покупки торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.\nТекст ошибки: {}." },
                    { 6, "Произошла ошибка при попытке продажи торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.\nТекст ошибки: {}." }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 6);

            migrationBuilder.AlterColumn<int>(
                name: "status",
                table: "t_request_log",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Request http status");

            migrationBuilder.AlterColumn<string>(
                name: "request_path",
                table: "t_request_log",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Request path");

            migrationBuilder.AlterColumn<string>(
                name: "request_method",
                table: "t_request_log",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldComment: "Request http method");

            migrationBuilder.AlterColumn<string>(
                name: "referer",
                table: "t_request_log",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Request referer");

            migrationBuilder.AlterColumn<string>(
                name: "login",
                table: "t_request_log",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255,
                oldNullable: true,
                oldComment: "Request user login");

            migrationBuilder.AlterColumn<string>(
                name: "ip",
                table: "t_request_log",
                type: "character varying(25)",
                maxLength: 25,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(25)",
                oldMaxLength: 25,
                oldNullable: true,
                oldComment: "Request ip address");

            migrationBuilder.AlterColumn<int>(
                name: "id_user",
                table: "t_request_log",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Request user id");

            migrationBuilder.AlterColumn<long>(
                name: "elapsed_ms",
                table: "t_request_log",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "Request lifetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "date",
                table: "t_request_log",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldComment: "Request date");

            migrationBuilder.AddColumn<string>(
                name: "ex_message",
                table: "t_request_log",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ex_stack",
                table: "t_request_log",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 3,
                column: "template",
                value: "Произошла ошибка при попытке покупки торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.\nТекст ошибки: {}.");

            migrationBuilder.UpdateData(
                table: "t_event_templates",
                keyColumn: "id",
                keyValue: 4,
                column: "template",
                value: "Произошла ошибка при попытке продажи торговой пары {} в количестве {} шт. по курсу {} USDT на сумму {} USDT. Дата: {} Время: {}.\nТекст ошибки: {}.");
        }
    }
}
