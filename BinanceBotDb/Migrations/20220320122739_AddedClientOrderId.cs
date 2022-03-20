using Microsoft.EntityFrameworkCore.Migrations;

namespace BinanceBotDb.Migrations
{
    public partial class AddedClientOrderId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "id_time_in_force",
                table: "t_orders");

            migrationBuilder.AlterColumn<int>(
                name: "id_type",
                table: "t_orders",
                type: "integer",
                nullable: false,
                comment: "1 - Limit\n2 - Market",
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "Order type id");

            migrationBuilder.AddColumn<string>(
                name: "client_order_id",
                table: "t_orders",
                type: "text",
                nullable: true,
                comment: "Exchange inner id");

            migrationBuilder.AddColumn<long>(
                name: "order_id",
                table: "t_orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                comment: "Exchange inner id");

            migrationBuilder.AddColumn<string>(
                name: "time_in_force",
                table: "t_orders",
                type: "text",
                nullable: true,
                comment: "GTC, IOC, FOK");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "client_order_id",
                table: "t_orders");

            migrationBuilder.DropColumn(
                name: "order_id",
                table: "t_orders");

            migrationBuilder.DropColumn(
                name: "time_in_force",
                table: "t_orders");

            migrationBuilder.AlterColumn<int>(
                name: "id_type",
                table: "t_orders",
                type: "integer",
                nullable: false,
                comment: "Order type id",
                oldClrType: typeof(int),
                oldType: "integer",
                oldComment: "1 - Limit\n2 - Market");

            migrationBuilder.AddColumn<int>(
                name: "id_time_in_force",
                table: "t_orders",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "1 - Full\n2 - Partial");
        }
    }
}
