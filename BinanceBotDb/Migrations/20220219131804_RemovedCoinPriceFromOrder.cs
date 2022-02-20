using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BinanceBotDb.Migrations
{
    public partial class RemovedCoinPriceFromOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "coin_price",
                table: "t_orders");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "t_orders",
                newName: "date_created");

            migrationBuilder.AddColumn<DateTime>(
                name: "date_closed",
                table: "t_orders",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Order closing date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_closed",
                table: "t_orders");

            migrationBuilder.RenameColumn(
                name: "date_created",
                table: "t_orders",
                newName: "date");

            migrationBuilder.AddColumn<double>(
                name: "coin_price",
                table: "t_orders",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                comment: "Current coin price (when order was created) in secondary asset of trading pair");
        }
    }
}
