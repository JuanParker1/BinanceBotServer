using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BinanceBotDb.Migrations
{
    public partial class ChangedOrderCloseDateToNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_closed",
                table: "t_orders",
                type: "timestamp without time zone",
                nullable: true,
                comment: "Order closing date",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldComment: "Order closing date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "date_closed",
                table: "t_orders",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Order closing date",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true,
                oldComment: "Order closing date");
        }
    }
}
