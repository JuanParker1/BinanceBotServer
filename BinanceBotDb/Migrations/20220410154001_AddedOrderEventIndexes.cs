using Microsoft.EntityFrameworkCore.Migrations;

namespace BinanceBotDb.Migrations
{
    public partial class AddedOrderEventIndexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_t_orders_date_closed",
                table: "t_orders",
                column: "date_closed");

            migrationBuilder.CreateIndex(
                name: "IX_t_orders_id_user",
                table: "t_orders",
                column: "id_user");

            migrationBuilder.CreateIndex(
                name: "IX_t_events_date",
                table: "t_events",
                column: "date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_t_orders_date_closed",
                table: "t_orders");

            migrationBuilder.DropIndex(
                name: "IX_t_orders_id_user",
                table: "t_orders");

            migrationBuilder.DropIndex(
                name: "IX_t_events_date",
                table: "t_events");
        }
    }
}
